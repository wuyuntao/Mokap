using GlmSharp;
using Mokap.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    sealed class BvhWriter2 : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region File Templates

        const string ROOT_START = @"HIERARCHY
ROOT {0}
{{
    OFFSET {1:f6} {2:f6} {3:f6}
    CHANNELS 3 Xposition Yposition Zposition Xrotation Yrotation Zrotation";

        const string JOINT_START = @"{4}JOINT {0}
{4}{{
{4}    OFFSET {1:f6} {2:f6} {3:f6}
{4}    CHANNELS 3 Xrotation Yrotation Zrotation";

        const string END = @"{3}END Site
{3}{{
    {3}OFFSET {0:f6} {1:f6} {2:f6}
{3}}}";

        const string JOINT_END = @"{0}}}";

        const string MOTION_START = @"MOTION
Frames: {0}
Frame Time: {1:f6}";

        #endregion

        private string filename;

        private MotionData motion;

        private TextWriter writer;

        BvhWriter2(string filename, MotionData motion)
        {
            this.filename = filename;
            this.motion = motion;

            writer = new StreamWriter(filename, false, Encoding.ASCII);
        }

        protected override void DisposeManaged()
        {
            SafeDispose(ref writer);

            base.DisposeManaged();
        }

        public static void Write(string filename, MotionData motion)
        {
            using (var writer = new BvhWriter2(filename, motion))
            {
                writer.Write();
            }
        }

        public static void Write(string filename, string recordFilename)
        {
            var reader = new RecordReader(recordFilename);
            var frames = (from msg in reader.ReadAllMessages()
                          where msg is BodyFrameData
                          select (BodyFrameData)msg into frame
                          where frame.Bodies.Any(b => b.IsTracked)
                          select frame).ToArray();

            var motion = MotionData.CreateData(frames[0]);
            foreach (var frame in frames)
            {
                motion.AppendFrame(frame);
            }

            Write(filename, motion);
        }

        private void Write()
        {
            // Root
            foreach (var body in motion.Body.Bodies)
            {
                var position = new Vector3D();  // TODO Position
                var rootStartString = string.Format(ROOT_START
                        , BoneType.Root
                        , position.X
                        , position.Y
                        , position.Z);

                writer.WriteLine(rootStartString);

                foreach (var boneDef in BoneDef.FindChildren(BoneType.Root))
                {
                    WriteBone(body, boneDef, 1);
                }

                writer.WriteLine(string.Format(JOINT_END, ""));
            }

            // Motion
            var motionString = string.Format(MOTION_START
                    , motion.FrameCount
                    , motion.TotalTime.TotalSeconds / motion.FrameCount);
            writer.WriteLine(motionString);

            // Motion Frames
            foreach (var frame in motion.Frames)
            {
                var values = new List<string>();

                foreach (var body in frame.Bodies)
                {
                    //var frame = motion.Skeleton.Frames[i];
                    //values.Add(frame.Offset.X.ToString("f4"));
                    //values.Add(frame.Offset.Y.ToString("f4"));
                    //values.Add(frame.Offset.Z.ToString("f4"));

                    //values.Add(frame.Rotation.X.ToString("f4"));
                    //values.Add(frame.Rotation.Y.ToString("f4"));
                    //values.Add(frame.Rotation.Z.ToString("f4"));

                    values.Add("0");
                    values.Add("0");
                    values.Add("0");

                    values.Add("0");
                    values.Add("0");
                    values.Add("0");

                    foreach (var boneDef in BoneDef.BonesByHierarchy)
                    {
                        if (boneDef.Type == BoneType.Spine || boneDef.Type == BoneType.Chest || boneDef.Type == BoneType.ShoulderLeft || boneDef.Type == BoneType.UpperArmLeft)
                        {
                            var bone = body.FindBone(boneDef.Type);

                            var axis = bone.Rotation.Axis;
                            var rotation = dquat.FromAxisAngle(bone.Rotation.Angle, new dvec3(axis.X, axis.Y, axis.Z));

                            if (boneDef.ParentType != BoneType.Root)
                            {
                                var pBone = body.FindBone(boneDef.ParentType);
                                var pAxis = pBone.Rotation.Axis;
                                var pRotation = dquat.FromAxisAngle(pBone.Rotation.Angle, new dvec3(pAxis.X, pAxis.Y, pAxis.Z));

                                rotation = pRotation.Inverse * rotation;
                            }

                            var eulerAngles = rotation.EulerAngles / Math.PI * 180;

                            values.Add(eulerAngles.x.ToString("f4"));
                            values.Add(eulerAngles.y.ToString("f4"));
                            values.Add(eulerAngles.z.ToString("f4"));
                        }
                        else
                        {
                            values.Add("0");
                            values.Add("0");
                            values.Add("0");
                        }
                    }
                }

                writer.WriteLine(string.Join(" ", values));
            }
        }

        private void WriteBone(MotionBodyData.Body body, BoneDef boneDef, int indent)
        {
            var bone = body.FindBone(boneDef.Type);
            var indentString = CreateIndent(indent);

            var offset = boneDef.ParentType == BoneType.Root
                    ? new Vector3D(0, 0, 0)
                    : new Vector3D(0, body.FindBone(boneDef.ParentType).Length, 0);

            var jointStartString = string.Format(JOINT_START
                , bone.Type
                , offset.X
                , offset.Y
                , offset.Z
                , indentString);
            writer.WriteLine(jointStartString);

            if (boneDef.IsEnd)
            {
                offset = new Vector3D(0, body.FindBone(boneDef.Type).Length, 0);
                var endString = string.Format(END
                    , offset.X
                    , offset.Y
                    , offset.Z
                    , CreateIndent(indent + 1));
                writer.WriteLine(endString);
            }
            else
            {
                foreach (var childBoneDef in BoneDef.FindChildren(boneDef.Type))
                {
                    WriteBone(body, childBoneDef, indent + 1);
                }
            }

            writer.WriteLine(string.Format(JOINT_END, indentString));
        }

        private string CreateIndent(int indent)
        {
            return string.Join("", Enumerable.Repeat("    ", indent));
        }
    }
}

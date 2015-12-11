using Mokap.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using JointType = Mokap.Schemas.RecorderMessages.JointType;

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
                        , JointType.SpineBase
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
                // TODO: Remove later
                if (frame.RelativeTime != TimeSpan.Parse("00:00:03.7347907"))
                    continue;

                var values = new List<string>();

                foreach (var body in frame.Bodies)
                {
                    {
                        values.Add(body.Position.X.ToString("f4"));
                        values.Add(body.Position.Y.ToString("f4"));
                        values.Add(body.Position.Z.ToString("f4"));

                        var eulerAngles = body.Rotation.ToEulerAngles();
                        values.Add(eulerAngles.X.ToString("f4"));
                        values.Add(eulerAngles.Y.ToString("f4"));
                        values.Add(eulerAngles.Z.ToString("f4"));
                    }

                    foreach (var boneDef in BoneDef.BonesByHierarchy)
                    {
                        var bone = body.FindBone(boneDef.Type);

                        var eulerAngles = bone.LocalRotation.ToEulerAngles();

                        values.Add(eulerAngles.X.ToString("f4"));
                        values.Add(eulerAngles.Y.ToString("f4"));
                        values.Add(eulerAngles.Z.ToString("f4"));
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
                    : BoneDef.Find(boneDef.ParentType).TPoseDirection3D * body.FindBone(boneDef.ParentType).Length;

            var jointStartString = string.Format(JOINT_START
                , boneDef.TailJointType
                , offset.X
                , offset.Y
                , offset.Z
                , indentString);
            writer.WriteLine(jointStartString);

            if (boneDef.IsEnd)
            {
                offset = boneDef.TPoseDirection3D * body.FindBone(boneDef.Type).Length;
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

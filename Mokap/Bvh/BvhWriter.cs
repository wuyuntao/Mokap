using Microsoft.Kinect;
using Mokap.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    class BvhWriter : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region File Templates

        const string ROOT_START = @"HIERARCHY
ROOT {0}
{{
    OFFSET {1:f6} {2:f6} {3:f6}
    CHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation";

        const string JOINT_START = @"{4}JOINT {0}
{4}{{
{4}    OFFSET {1:f6} {2:f6} {3:f6}
{4}    CHANNELS 3 Zrotation Xrotation Yrotation";

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

        private Motion motion;

        private StreamWriter writer;

        BvhWriter(string filename, Motion motion)
        {
            this.filename = filename;
            this.motion = motion;
            this.writer = new StreamWriter(filename, false, Encoding.ASCII);
        }

        public static void Write(string filename, Motion motion)
        {
            using (var writer = new BvhWriter(filename, motion))
            {
                writer.Write();
            }
        }

        protected override void DisposeManaged()
        {
            SafeDispose(ref this.writer);

            base.DisposeManaged();
        }

        private void Write()
        {
            // Root
            var position = this.motion.Skeleton.InitialPosition;
            var rootStartString = string.Format(ROOT_START
                    , JointType.SpineBase
                    , position.X
                    , position.Y - Settings.Default.SpineBaseBoneLength
                    , position.Z);

            this.writer.WriteLine(rootStartString);

            foreach (var bone in this.motion.Skeleton.Children)
            {
                WriteBone(bone, 1);
            }

            this.writer.WriteLine(string.Format(JOINT_END, ""));

            // Motion
            var motionString = string.Format(MOTION_START
                    , this.motion.FrameCount
                    , this.motion.FrameTime);
            this.writer.WriteLine(motionString);

            // Motion Frames
            for (int i = 0; i < motion.FrameCount; ++i)
            {
                var values = new List<string>();

                var frame = motion.Skeleton.Frames[i];
                values.Add(frame.Offset.X.ToString("f4"));
                values.Add(frame.Offset.Y.ToString("f4"));
                values.Add(frame.Offset.Z.ToString("f4"));

                values.Add(frame.Rotation.X.ToString("f4"));
                values.Add(frame.Rotation.Y.ToString("f4"));
                values.Add(frame.Rotation.Z.ToString("f4"));

                foreach (var bone in motion.Skeleton.Descendants)
                {
                    frame = bone.Frames[i];

                    values.Add(frame.Rotation.X.ToString("f4"));
                    values.Add(frame.Rotation.Y.ToString("f4"));
                    values.Add(frame.Rotation.Z.ToString("f4"));
                }

                this.writer.WriteLine(string.Join(" ", values));
            }
        }

        private void WriteBone(Bone bone, int indent)
        {
            var indentString = CreateIndent(indent);

            var offset = bone.Parent == null
                    ? new Vector3D(0, Settings.Default.SpineBaseBoneLength, 0)
                    : bone.Parent.InitialOffset;

            var jointStartString = string.Format(JOINT_START
                , bone.Name
                , offset.X
                , offset.Y
                , offset.Z
                , indentString);
            this.writer.WriteLine(jointStartString);

            if (bone.IsEnd)
            {
                offset = bone.InitialOffset;
                var endString = string.Format(END
                    , offset.X
                    , offset.Y
                    , offset.Z
                    , CreateIndent(indent + 1));
                this.writer.WriteLine(endString);
            }
            else
            {
                foreach (var child in bone.Children)
                {
                    WriteBone(child, indent + 1);
                }
            }

            this.writer.WriteLine(string.Format(JOINT_END, indentString));
        }

        private string CreateIndent(int indent)
        {
            return string.Join("", Enumerable.Repeat("    ", indent));
        }
    }
}
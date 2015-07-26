using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text;

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

        const string JOINT_END = @"}";

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
            this.writer = new StreamWriter(filename, false, Encoding.UTF8);
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
            var root = this.motion.Skeleton.Root;
            var rootStartString = string.Format(ROOT_START
                    , root.Type
                    , root.Offset.X
                    , root.Offset.Y
                    , root.Offset.Z);

            this.writer.WriteLine(rootStartString);

            WriteJointChildren(root, 1);

            this.writer.WriteLine(JOINT_END);

            // Motion
            var motionString = string.Format(MOTION_START
                    , this.motion.FrameCount
                    , this.motion.FrameTime);
            this.writer.WriteLine(motionString);

            // Motion Frames
            foreach (var frame in motion.Frames)
            {
                var frameString = string.Join(" ", Array.ConvertAll(frame.Values, v => v.ToString("f6")));
                this.writer.WriteLine(frameString);
            }
        }

        private void WriteJointChildren(Joint joint, int indent)
        {
            var jointStartString = string.Format(JOINT_START
                , joint.Type
                , joint.Offset.X
                , joint.Offset.Y
                , joint.Offset.Z
                , CreateIndent(indent));
            this.writer.WriteLine(jointStartString);

            if (joint.IsEnd)
            {
                var endString = string.Format(END
                    , joint.Offset.X
                    , joint.Offset.Y
                    , joint.Offset.Z
                    , CreateIndent(indent + 1));
                this.writer.WriteLine(endString);
            }
            else
            {
                foreach (var child in joint.Children)
                {
                    WriteJointChildren(child, indent + 1);
                }
            }

            this.writer.WriteLine(JOINT_END);
        }

        private string CreateIndent(int indent)
        {
            return string.Join("", Enumerable.Repeat("    ", indent));
        }
    }
}

using System;

namespace Mokap.Bvh
{
    class Motion
    {
        private Skeleton skeleton;
        private DateTime? startTime;
        private DateTime? endTime;

        public Motion(IBodyAdapter body)
        {
            this.skeleton = new Skeleton(body);

            this.startTime = DateTime.Now;
        }

        public void AppendFrame(IBodyAdapter body)
        {
            this.skeleton.AppendFrame(body);

            this.endTime = DateTime.Now;
        }

        #region Properties

        public Skeleton Skeleton
        {
            get { return this.skeleton; }
        }

        public bool HasSkeleton
        {
            get { return this.skeleton != null; }
        }

        public int FrameCount
        {
            get { return this.skeleton.Frames.Count; }
        }

        public double FrameTime
        {
            get
            {
                if (FrameCount == 0)
                    return 0;

                return (this.endTime.Value - this.startTime.Value).TotalSeconds / FrameCount;
            }
        }

        #endregion
    }
}
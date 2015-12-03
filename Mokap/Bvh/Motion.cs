using Mokap.Data;
using System;

namespace Mokap.Bvh
{
    class Motion
    {
        private Skeleton skeleton;
        private DateTime? startTime;
        private DateTime? endTime;

        public Motion(BodyFrameData.Body body)
        {
            skeleton = new Skeleton(body);

            startTime = DateTime.Now;
        }

        public void AppendFrame(BodyFrameData.Body body)
        {
            skeleton.AppendFrame(body);

            endTime = DateTime.Now;
        }

        #region Properties

        public Skeleton Skeleton
        {
            get { return skeleton; }
        }

        public bool HasSkeleton
        {
            get { return skeleton != null; }
        }

        public int FrameCount
        {
            get { return skeleton.Frames.Count; }
        }

        public double FrameTime
        {
            get
            {
                if (FrameCount == 0)
                    return 0;

                return (endTime.Value - startTime.Value).TotalSeconds / FrameCount;
            }
        }

        #endregion
    }
}
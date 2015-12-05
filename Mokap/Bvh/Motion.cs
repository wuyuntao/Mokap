using Mokap.Data;
using System;

namespace Mokap.Bvh
{
    class Motion
    {
        private Skeleton skeleton;
        private TimeSpan? startTime;
        private TimeSpan? endTime;

        public Motion(BodyFrameData.Body body, TimeSpan time)
        {
            skeleton = new Skeleton(body);

            startTime = time;
        }

        public void AppendFrame(BodyFrameData.Body body, TimeSpan time)
        {
            skeleton.AppendFrame(body);

            endTime = time;
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
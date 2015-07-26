using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mokap.Bvh
{
    class Motion
    {
        private Skeleton skeleton;
        private List<Frame> frames = new List<Frame>();
        private DateTime? startTime;
        private DateTime? endTime;

        public void CreateSkeleton(IBodyAdapter body)
        {
            this.skeleton = Skeleton.Create(body);

            this.startTime = DateTime.Now;
        }

        public void AppendFrame(IBodyAdapter body)
        {
            this.frames.Add(this.skeleton.CreateFrame(body));

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
            get { return this.frames.Count; }
        }

        public double FrameTime
        {
            get
            {
                if (this.frames.Count == 0)
                    return 0;

                return (this.endTime.Value - this.startTime.Value).TotalSeconds / this.frames.Count;
            }
        }

        public IEnumerable<Frame> Frames
        {
            get { return this.frames; }
        }

        #endregion
    }
}
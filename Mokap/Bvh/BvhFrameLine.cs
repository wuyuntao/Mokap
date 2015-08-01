using System.Collections.Generic;

namespace Mokap.Bvh
{
    class BvhFrameLine
    {
        List<BvhFrame> frames = new List<BvhFrame>();

        public void Add(BvhFrame frame)
        {
            this.frames.Add(frame);
        }

        public BvhFrame this[int i]
        {
            get { return this.frames[i]; }
        }

        public int Count
        {
            get { return this.frames.Count; }
        }
    }
}
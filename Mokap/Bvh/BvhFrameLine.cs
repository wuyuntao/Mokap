using System.Collections.Generic;

namespace Mokap.Bvh
{
    class BvhFrameLine
    {
        List<BvhFrame> frames = new List<BvhFrame>();

        public void Add(BvhFrame frame)
        {
            frames.Add(frame);
        }

        public BvhFrame this[int i]
        {
            get { return frames[i]; }
        }

        public int Count
        {
            get { return frames.Count; }
        }
    }
}
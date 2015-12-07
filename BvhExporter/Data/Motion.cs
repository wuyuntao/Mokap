using System;
using System.Collections.Generic;

namespace BvhExporter
{
    [Serializable]
    class Motion
    {
        public List<Bone> Bones = new List<Bone>();

        public List<Frame> Frames = new List<Frame>();
    }
}

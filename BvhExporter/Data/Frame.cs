using System;
using System.Collections.Generic;

namespace BvhExporter
{
    [Serializable]
    class Frame
    {
        public long Time;

        public List<Bone> Bones = new List<Bone>();
    }
}

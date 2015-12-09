using GlmSharp;
using System;

namespace BvhExporter
{
    [Serializable]
    class Bone
    {
        public string Name;

        public string ParentName;

        public dvec3 HeadPos;

        public dvec3 TailPos;

        public dquat Rotation;

        public dvec3 RotationAngles;
    }
}

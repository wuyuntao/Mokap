using GlmSharp;
using System;

namespace BvhExporter
{
    [Serializable]
    class Bone
    {
        public string Name;

        public string ParentName;

        public vec3 HeadPos;

        public vec3 TailPos;

        public quat Rotation;

        public vec3 RotationAngles;
    }
}

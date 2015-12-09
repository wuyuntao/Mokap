using System;
using System.Linq;

namespace Mokap.Data
{
    [Serializable]
    sealed class MotionFrameData
    {
        public TimeSpan RelativeTime;

        public Body[] Bodies;

        public static MotionFrameData CreateData(BodyFrameData bodyFrame)
        {
            return new MotionFrameData()
            {
                RelativeTime = bodyFrame.RelativeTime,
                Bodies = (from b in bodyFrame.Bodies
                          where b.IsTracked
                          select CreateBodyData(b)).ToArray(),
            };
        }

        private static Body CreateBodyData(BodyFrameData.Body input)
        {
            return new Body()
            {
                TrackingId = input.TrackingId,
                Bones = Array.ConvertAll(BoneDef.Bones, CreateBoneData),
            };
        }

        private static Bone CreateBoneData(BoneDef def)
        {
            return new Bone()
            {
                Type = def.Type,
            };
        }

        [Serializable]
        public sealed class Body
        {
            public ulong TrackingId;

            public Bone[] Bones;
        }

        [Serializable]
        public sealed class Bone
        {
            public BoneType Type;
        }
    }
}

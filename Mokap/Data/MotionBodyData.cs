using System;
using System.Linq;

namespace Mokap.Data
{
    [Serializable]
    sealed class MotionBodyData
    {
        public Body[] Bodies;

        public static MotionBodyData CreateData(BodyFrameData bodyFrame)
        {
            return new MotionBodyData()
            {
                Bodies = (from b in bodyFrame.Bodies
                          where b.IsTracked
                          select CreateBodyData(b)).ToArray(),
            };
        }

        public static Body CreateBodyData(BodyFrameData.Body input)
        {
            return new Body()
            {
                TrackingId = input.TrackingId,
                Bones = BoneDef.Bones.Select(def => CreateBoneData(input, def)).ToArray(),
            };
        }

        private static Bone CreateBoneData(BodyFrameData.Body body, BoneDef boneDef)
        {
            var length = CalculateBoneLength(body, boneDef);

            if (boneDef.MirrorType != boneDef.Type)
            {
                var mirrorDef = BoneDef.Find(boneDef.MirrorType);
                var mirrorLength = CalculateBoneLength(body, mirrorDef);

                length = (length + mirrorLength) / 2;
            }

            return new Bone()
            {
                Type = boneDef.Type,
                Length = length,
            };
        }

        private static double CalculateBoneLength(BodyFrameData.Body body, BoneDef boneDef)
        {
            var headPosition = body.Joints[boneDef.HeadJointType].Position3D;
            var tailPosition = body.Joints[boneDef.TailJointType].Position3D;

            return (headPosition - tailPosition).Length;
        }

        [Serializable]
        public sealed class Body
        {
            public ulong TrackingId;

            public Bone[] Bones;

            public Bone FindBone(BoneType type)
            {
                return Bones[(int)type];
            }
        }

        [Serializable]
        public sealed class Bone
        {
            public BoneType Type;

            public double Length;
        }
    }
}

using System;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Mokap.Data
{
    [Serializable]
    sealed class MotionFrameData
    {
        public TimeSpan RelativeTime;

        public Body[] Bodies;

        public static MotionFrameData CreateData(BodyFrameData bodyFrame, MotionBodyData.Body bodyDef)
        {
            return new MotionFrameData()
            {
                RelativeTime = bodyFrame.RelativeTime,
                Bodies = (from b in bodyFrame.Bodies
                          where b.IsTracked
                          select CreateBodyData(b, bodyDef)).ToArray(),
            };
        }

        public static Body CreateBodyData(BodyFrameData.Body input, MotionBodyData.Body bodyDef)
        {
            var body = new Body()
            {
                TrackingId = input.TrackingId,
                Bones = new Bone[BoneDef.BoneCount],
            };

            foreach (var boneDef in BoneDef.BonesByHierarchy)
            {
                body.Bones[(int)boneDef.Type] = CreateBoneData(body, boneDef, input, bodyDef);
            }

            return body;
        }

        private static Bone CreateBoneData(Body body, BoneDef boneDef, BodyFrameData.Body input, MotionBodyData.Body bodyDef)
        {
            var bone = new Bone() { Type = boneDef.Type };

            // Head position
            if (boneDef.ParentType == BoneType.Root)
            {
                bone.HeadPosition = input.Joints[boneDef.HeadJointType].Position3D;
            }
            else
            {
                bone.HeadPosition = body.FindBone(boneDef.ParentType).TailPosition;
            }

            // Tail position
            var boneLength = bodyDef.FindBone(boneDef.Type).Length;
            var rawHeadPosition = input.Joints[boneDef.HeadJointType].Position3D;
            var rawTailPosition = input.Joints[boneDef.TailJointType].Position3D;
            var direction = rawTailPosition - rawHeadPosition;
            direction.Normalize();

            bone.TailPosition = bone.HeadPosition + direction * boneLength;

            // Rotation
            if (boneDef.IsEnd)
            {
                var upward = new Vector3D(0, 1, 0);
                bone.Rotation = KinectHelper.LookRotation(rawTailPosition - rawHeadPosition, upward);
            }
            else
            {
                bone.Rotation = input.Joints[boneDef.TailJointType].Rotation;
            }

            return bone;
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

            public Vector3D HeadPosition;

            public Vector3D TailPosition;

            public Quaternion Rotation;
        }
    }
}

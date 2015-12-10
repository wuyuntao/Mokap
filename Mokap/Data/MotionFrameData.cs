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

        public static MotionFrameData CreateData(BodyFrameData bodyFrame, MotionBodyData bodyDefs)
        {
            return new MotionFrameData()
            {
                RelativeTime = bodyFrame.RelativeTime,
                Bodies = bodyFrame.Bodies
                            .Where(b => b.IsTracked)
                            .Select(b =>
                            {
                                var bodyDef = Array.Find(bodyDefs.Bodies, bd => bd.TrackingId == b.TrackingId);

                                return CreateBodyData(b, bodyDef);
                            }).ToArray(),
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
            var bone = new Bone(body, boneDef.Type);

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
            private Body body;

            public BoneType Type;

            public Vector3D HeadPosition;

            public Vector3D TailPosition;

            public Quaternion Rotation;

            public Bone(Body body, BoneType boneType)
            {
                this.body = body;
                Type = boneType;
            }

            public Bone Parent
            {
                get
                {
                    var boneDef = BoneDef.Find(Type);

                    return boneDef.ParentType == BoneType.Root ? null : body.FindBone(boneDef.ParentType);
                }
            }

            public Vector3D LocalHeadPosition
            {
                get { return Parent != null ? HeadPosition - Parent.HeadPosition : HeadPosition; }
            }

            public Vector3D LocalTailPosition
            {
                get { return Parent != null ? TailPosition - Parent.HeadPosition : TailPosition; }
            }

            public Quaternion LocalRotation
            {
                get
                {
                    if (Parent == null)
                    {
                        return Rotation;
                    }
                    else
                    {
                        var parentRotation = new Quaternion(Parent.Rotation.X, Parent.Rotation.Y, Parent.Rotation.W, Parent.Rotation.Z);
                        parentRotation.Invert();

                        return parentRotation * Rotation;
                    }
                }
            }
        }
    }
}

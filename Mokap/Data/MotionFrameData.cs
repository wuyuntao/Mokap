using NLog;
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
                Position = input.Joints[Schemas.RecorderMessages.JointType.SpineBase].Position3D,
                Rotation = input.Joints[Schemas.RecorderMessages.JointType.SpineBase].Rotation,
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
                bone.Rotation = QuaternionHelper.LookRotation(rawTailPosition - rawHeadPosition, upward);
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

            public Vector3D Position;

            public Quaternion Rotation;

            public Bone[] Bones;

            public Bone FindBone(BoneType type)
            {
                return Bones[(int)type];
            }
        }

        [Serializable]
        public sealed class Bone
        {
            private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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

            public Quaternion LocalRotation
            {
                get
                {
                    var boneDef = BoneDef.Find(Type);

                    if (Parent == null)
                    {
                        var inversedParentRotation = body.Rotation.Copy();
                        inversedParentRotation.Invert();

                        var inversedTPoseRotation = boneDef.TPoseRotation.Copy();
                        inversedTPoseRotation.Invert();

                        var localRotation = Quaternion.Identity * inversedParentRotation * Rotation * inversedTPoseRotation;

                        // TODO: Remove later
                        logger.Trace("{0} {1} = {2} = {3} * {4} * {5} * {6}",
                            boneDef.TailJointType,
                            localRotation.ToEulerAngles().ToString("f3"),
                            localRotation.ToString("f3"),
                            Quaternion.Identity.ToString("f3"),
                            inversedParentRotation.ToString("f3"),
                            Rotation.ToString("f3"),
                            inversedTPoseRotation.ToString("f3"));

                        return localRotation;
                    }
                    else
                    {
                        var parentBoneDef = BoneDef.Find(boneDef.ParentType);

                        var inversedParentRotation = Parent.Rotation.Copy();
                        inversedParentRotation.Invert();

                        var inversedTPoseRotation = boneDef.TPoseRotation.Copy();
                        inversedTPoseRotation.Invert();

                        var localRotation = parentBoneDef.TPoseRotation * inversedParentRotation * Rotation * inversedTPoseRotation;

                        // TODO: Remove later

                        logger.Trace("{0} {1} = {2} = {3} * {4} * {5} * {6}",
                            boneDef.TailJointType,
                            localRotation.ToEulerAngles().ToString("f3"),
                            localRotation.ToString("f3"),
                            parentBoneDef.TPoseRotation.ToString("f3"),
                            inversedParentRotation.ToString("f3"),
                            Rotation.ToString("f3"),
                            inversedTPoseRotation.ToString("f3"));

                        return localRotation;
                    }
                }
            }
        }
    }
}

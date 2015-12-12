using Mokap.Schemas.RecorderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Mokap.Data
{

    [Serializable]
    sealed class BoneDef
    {
        public BoneType Type;

        public BoneType ParentType;

        public BoneType MirrorType;

        public JointType HeadJointType;

        public JointType TailJointType;

        public BoneDirection TPoseDirection;

        public bool IsEnd;

        public Quaternion TPoseRotation
        {
            get
            {
                if (ParentType == BoneType.Root)
                {
                    return LocalTPoseRotation;
                }
                else
                {
                    var parentBoneDef = BoneDef.Find(ParentType);

                    return parentBoneDef.TPoseRotation * LocalTPoseRotation;
                }
            }
        }

        public Quaternion LocalTPoseRotation
        {
            get
            {
                switch (TailJointType)
                {
                    case JointType.ShoulderLeft:
                    case JointType.HipLeft:
                    case JointType.KneeLeft:
                        return new Quaternion(new Vector3D(0, 0, 1), 90);

                    case JointType.ShoulderRight:
                    case JointType.HipRight:
                    case JointType.KneeRight:
                        return new Quaternion(new Vector3D(0, 0, 1), -90);

                    case JointType.FootLeft:
                    case JointType.FootRight:
                    case JointType.ThumbLeft:
                    case JointType.ThumbRight:
                        return new Quaternion(new Vector3D(1, 0, 0), 90);

                    default:
                        return Quaternion.Identity;
                }
            }
        }

        public Vector3D TPoseDirection3D
        {
            get
            {
                switch (TPoseDirection)
                {
                    case BoneDirection.Up:
                        return new Vector3D(0, 1, 0);

                    case BoneDirection.Down:
                        return new Vector3D(0, -1, 0);

                    case BoneDirection.Left:
                        return new Vector3D(-1, 0, 0);

                    case BoneDirection.Right:
                        return new Vector3D(1, 0, 0);

                    case BoneDirection.Forward:
                        return new Vector3D(0, 0, 1);

                    case BoneDirection.Backward:
                        return new Vector3D(0, 0, -1);

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private BoneDef(BoneType type, BoneType parentType, BoneType mirrorType, JointType headJointType, JointType tailJointType, BoneDirection tPoseDirection, bool isEnd)
        {
            Type = type;
            ParentType = parentType;
            MirrorType = mirrorType;
            HeadJointType = headJointType;
            TailJointType = tailJointType;
            IsEnd = isEnd;
            TPoseDirection = tPoseDirection;
        }

        private BoneDef(BoneType type, BoneType parentType, BoneType mirrorType, JointType headJointType, JointType tailJointType, BoneDirection tPoseDirection)
            : this(type, parentType, mirrorType, headJointType, tailJointType, tPoseDirection, false)
        {
        }

        private BoneDef(BoneType type, BoneType parentType, JointType headJointType, JointType tailJointType, BoneDirection tPoseDirection, bool isEnd)
            : this(type, parentType, type, headJointType, tailJointType, tPoseDirection, isEnd)
        {
        }

        private BoneDef(BoneType type, BoneType parentType, JointType headJointType, JointType tailJointType, BoneDirection tPoseDirection)
            : this(type, parentType, headJointType, tailJointType, tPoseDirection, false)
        {
        }

        private static readonly BoneDef[] bones = new[]
        {
            // Torso
            new BoneDef(BoneType.Head, BoneType.Neck, JointType.Neck, JointType.Head, BoneDirection.Up, true),
            new BoneDef(BoneType.Neck, BoneType.Chest, JointType.SpineShoulder, JointType.Neck, BoneDirection.Up),
            new BoneDef(BoneType.Chest, BoneType.Spine, JointType.SpineMid, JointType.SpineShoulder, BoneDirection.Up),
            new BoneDef(BoneType.Spine, BoneType.Root, JointType.SpineBase, JointType.SpineMid, BoneDirection.Up),
            
            // Right Arm
            new BoneDef(BoneType.ShoulderRight, BoneType.Chest, BoneType.ShoulderLeft, JointType.SpineShoulder, JointType.ShoulderRight, BoneDirection.Right),
            new BoneDef(BoneType.UpperArmRight, BoneType.ShoulderRight, BoneType.UpperArmLeft, JointType.ShoulderRight, JointType.ElbowRight, BoneDirection.Right),
            new BoneDef(BoneType.ForearmRight, BoneType.UpperArmRight, BoneType.ForearmLeft, JointType.ElbowRight, JointType.WristRight, BoneDirection.Right),
            new BoneDef(BoneType.HandRight, BoneType.ForearmRight, BoneType.HandLeft, JointType.WristRight, JointType.HandRight, BoneDirection.Right),
            new BoneDef(BoneType.HandTipRight, BoneType.HandRight, BoneType.HandTipLeft, JointType.HandRight, JointType.HandTipRight, BoneDirection.Right, true),
            new BoneDef(BoneType.ThumbRight, BoneType.HandRight, BoneType.ThumbLeft, JointType.HandRight, JointType.ThumbRight, BoneDirection.Forward, true),

            // Left Arm
            new BoneDef(BoneType.ShoulderLeft, BoneType.Chest, BoneType.ShoulderRight, JointType.SpineShoulder, JointType.ShoulderLeft, BoneDirection.Left),
            new BoneDef(BoneType.UpperArmLeft, BoneType.ShoulderLeft, BoneType.UpperArmRight, JointType.ShoulderLeft, JointType.ElbowLeft, BoneDirection.Left),
            new BoneDef(BoneType.ForearmLeft, BoneType.UpperArmLeft, BoneType.ForearmRight, JointType.ElbowLeft, JointType.WristLeft, BoneDirection.Left),
            new BoneDef(BoneType.HandLeft, BoneType.ForearmLeft, BoneType.HandRight, JointType.WristLeft, JointType.HandLeft, BoneDirection.Left),
            new BoneDef(BoneType.HandTipLeft, BoneType.HandLeft, BoneType.HandTipRight, JointType.HandLeft, JointType.HandTipLeft, BoneDirection.Left, true),
            new BoneDef(BoneType.ThumbLeft, BoneType.HandLeft, BoneType.ThumbRight, JointType.HandLeft, JointType.ThumbLeft, BoneDirection.Forward, true),
            
            // Right Leg
            new BoneDef(BoneType.HipRight, BoneType.Root, BoneType.HipRight, JointType.SpineBase, JointType.HipRight, BoneDirection.Right),
            new BoneDef(BoneType.ThighRight, BoneType.HipRight, BoneType.ThighLeft, JointType.HipRight, JointType.KneeRight, BoneDirection.Down),
            new BoneDef(BoneType.ShinRight, BoneType.ThighRight, BoneType.ShinLeft, JointType.KneeRight, JointType.AnkleRight, BoneDirection.Down),
            new BoneDef(BoneType.FootRight, BoneType.ShinRight, BoneType.FootLeft, JointType.AnkleRight, JointType.FootRight, BoneDirection.Forward, true),
            
            // Left Leg
            new BoneDef(BoneType.HipLeft, BoneType.Root, BoneType.HipRight, JointType.SpineBase, JointType.HipLeft, BoneDirection.Left),
            new BoneDef(BoneType.ThighLeft, BoneType.HipLeft, BoneType.ThighRight, JointType.HipLeft, JointType.KneeLeft, BoneDirection.Down),
            new BoneDef(BoneType.ShinLeft, BoneType.ThighLeft, BoneType.ShinRight, JointType.KneeLeft, JointType.AnkleLeft, BoneDirection.Down),
            new BoneDef(BoneType.FootLeft, BoneType.ShinLeft, BoneType.FootRight, JointType.AnkleLeft, JointType.FootLeft, BoneDirection.Forward, true),
        };

        public static BoneDef Find(BoneType type)
        {
            return Array.Find(bones, b => b.Type == type);
        }

        public static IEnumerable<BoneDef> FindChildren(BoneType type)
        {
            return bones.Where(b => b.ParentType == type);
        }

        public static IEnumerable<BoneDef> FindDescendants(BoneType type)
        {
            var children = FindChildren(type);

            foreach (var child in children)
            {
                yield return child;

                var descendants = FindDescendants(child.Type);

                foreach (var descendant in descendants)
                {
                    yield return descendant;
                }
            }
        }

        public static IEnumerable<BoneDef> Bones
        {
            get { return bones; }
        }

        public static IEnumerable<BoneDef> BonesByHierarchy
        {
            get { return FindDescendants(BoneType.Root); }
        }

        public static int BoneCount
        {
            get { return bones.Length; }
        }
    }
}

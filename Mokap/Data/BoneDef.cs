using Mokap.Schemas.RecorderMessages;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool IsEnd;

        private BoneDef(BoneType type, BoneType parentType, BoneType mirrorType, JointType headJointType, JointType tailJointType, bool isEnd)
        {
            Type = type;
            ParentType = parentType;
            MirrorType = mirrorType;
            HeadJointType = headJointType;
            TailJointType = tailJointType;
            IsEnd = isEnd;
        }

        private BoneDef(BoneType type, BoneType parentType, BoneType mirrorType, JointType headJointType, JointType tailJointType)
            :this(type, parentType, mirrorType, headJointType, tailJointType, false)
        {
        }

        private BoneDef(BoneType type, BoneType parentType, JointType headJointType, JointType tailJointType, bool isEnd)
            : this(type, parentType, type, headJointType, tailJointType, isEnd)
        {
        }

        private BoneDef(BoneType type, BoneType parentType, JointType headJointType, JointType tailJointType)
            : this(type, parentType, headJointType, tailJointType, false)
        {
        }

        private static readonly BoneDef[] bones = new[]
        {
            // Torso
            new BoneDef(BoneType.Head, BoneType.Neck, JointType.Neck, JointType.Head, true),
            new BoneDef(BoneType.Neck, BoneType.Chest, JointType.SpineShoulder, JointType.Neck),
            new BoneDef(BoneType.Chest, BoneType.Spine, JointType.SpineMid, JointType.SpineShoulder),
            new BoneDef(BoneType.Spine, BoneType.Root, JointType.SpineBase, JointType.SpineMid),
            
            // Right Arm
            new BoneDef(BoneType.ShoulderRight, BoneType.Chest, BoneType.ShoulderLeft, JointType.SpineShoulder, JointType.ShoulderRight),
            new BoneDef(BoneType.UpperArmRight, BoneType.ShoulderRight, BoneType.UpperArmLeft, JointType.ShoulderRight, JointType.ElbowRight),
            new BoneDef(BoneType.ForearmRight, BoneType.UpperArmRight, BoneType.ForearmLeft, JointType.ElbowRight, JointType.WristRight),
            new BoneDef(BoneType.HandRight, BoneType.ForearmRight, BoneType.HandLeft, JointType.WristRight, JointType.HandRight),
            new BoneDef(BoneType.HandTipRight, BoneType.HandRight, BoneType.HandTipLeft, JointType.HandRight, JointType.HandTipRight, true),
            new BoneDef(BoneType.ThumbRight, BoneType.HandRight, BoneType.ThumbLeft, JointType.HandRight, JointType.ThumbRight, true),

            // Left Arm
            new BoneDef(BoneType.ShoulderLeft, BoneType.Chest, BoneType.ShoulderRight, JointType.SpineShoulder, JointType.ShoulderLeft),
            new BoneDef(BoneType.UpperArmLeft, BoneType.ShoulderLeft, BoneType.UpperArmRight, JointType.ShoulderLeft, JointType.ElbowLeft),
            new BoneDef(BoneType.ForearmLeft, BoneType.UpperArmLeft, BoneType.ForearmRight, JointType.ElbowLeft, JointType.WristLeft),
            new BoneDef(BoneType.HandLeft, BoneType.ForearmLeft, BoneType.HandRight, JointType.WristLeft, JointType.HandLeft),
            new BoneDef(BoneType.HandTipLeft, BoneType.HandLeft, BoneType.HandTipRight, JointType.HandLeft, JointType.HandTipLeft, true),
            new BoneDef(BoneType.ThumbLeft, BoneType.HandLeft, BoneType.ThumbRight, JointType.HandLeft, JointType.ThumbLeft, true),
            
            // Right Leg
            new BoneDef(BoneType.HipRight, BoneType.Root, BoneType.HipRight, JointType.SpineBase, JointType.HipRight),
            new BoneDef(BoneType.ThighRight, BoneType.HipRight, BoneType.ThighLeft, JointType.HipRight, JointType.KneeRight),
            new BoneDef(BoneType.ShinRight, BoneType.ThighRight, BoneType.ShinLeft, JointType.KneeRight, JointType.AnkleRight),
            new BoneDef(BoneType.FootRight, BoneType.ShinRight, BoneType.FootLeft, JointType.AnkleRight, JointType.FootRight, true),
            
            // Left Leg
            new BoneDef(BoneType.HipLeft, BoneType.Root, BoneType.HipRight, JointType.SpineBase, JointType.HipLeft),
            new BoneDef(BoneType.ThighLeft, BoneType.HipLeft, BoneType.ThighRight, JointType.HipLeft, JointType.KneeLeft),
            new BoneDef(BoneType.ShinLeft, BoneType.ThighLeft, BoneType.ShinRight, JointType.KneeLeft, JointType.AnkleLeft),
            new BoneDef(BoneType.FootLeft, BoneType.ShinLeft, BoneType.FootRight, JointType.AnkleLeft, JointType.FootLeft, true),
        };

        public static BoneDef Find(BoneType type)
        {
            return Array.Find(bones, b => b.Type == type);
        }

        private static IEnumerable<BoneDef> FindChildren(BoneType type)
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

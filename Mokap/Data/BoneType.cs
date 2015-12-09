using Mokap.Schemas.RecorderMessages;
using System;

namespace Mokap.Data
{
    enum BoneType
    {
        // Torso
        Root,
        Head,
        Neck,
        Chest,
        Spine,

        // Right Arm
        ShoulderRight,
        UpperArmRight,
        ForearmRight,
        HandRight,
        HandTipRight,
        ThumbRight,

        // Left Arm
        ShoulderLeft,
        UpperArmLeft,
        ForearmLeft,
        HandLeft,
        HandTipLeft,
        ThumbLeft,

        // Right Leg
        HipRight,
        ThighRight,
        ShinRight,
        FootRight,

        // Left Leg
        HipLeft,
        ThighLeft,
        ShinLeft,
        FootLeft,
    }

    [Serializable]
    sealed class BoneDef
    {
        public BoneType Type;

        public BoneType ParentType;

        public JointType StartJointType;

        public JointType EndJointType;

        private BoneDef(BoneType type, BoneType parentType, JointType startJointType, JointType endJointType)
        {
            Type = type;
            ParentType = parentType;
            StartJointType = startJointType;
            EndJointType = endJointType;
        }

        public static readonly BoneDef[] Bones = new[]
        {
            // Torso
            new BoneDef(BoneType.Head, BoneType.Neck, JointType.Neck, JointType.Head),
            new BoneDef(BoneType.Neck, BoneType.Chest, JointType.SpineShoulder, JointType.Neck),
            new BoneDef(BoneType.Chest, BoneType.Spine, JointType.SpineMid, JointType.SpineShoulder),
            new BoneDef(BoneType.Spine, BoneType.Root, JointType.SpineBase, JointType.SpineMid),
            
            // Right Arm
            new BoneDef(BoneType.ShoulderRight, BoneType.Chest, JointType.SpineShoulder, JointType.ShoulderRight),
            new BoneDef(BoneType.UpperArmRight, BoneType.ShoulderRight, JointType.ShoulderRight, JointType.ElbowRight),
            new BoneDef(BoneType.ForearmRight, BoneType.UpperArmRight, JointType.ElbowRight, JointType.WristRight),
            new BoneDef(BoneType.HandRight, BoneType.ForearmRight, JointType.WristRight, JointType.HandRight),
            new BoneDef(BoneType.HandTipRight, BoneType.HandRight, JointType.HandRight, JointType.HandTipRight),
            new BoneDef(BoneType.ThumbRight, BoneType.HandRight, JointType.HandRight, JointType.ThumbRight),

            // Left Arm
            new BoneDef(BoneType.ShoulderLeft, BoneType.Chest, JointType.SpineShoulder, JointType.ShoulderLeft),
            new BoneDef(BoneType.UpperArmLeft, BoneType.ShoulderLeft, JointType.ShoulderLeft, JointType.ElbowLeft),
            new BoneDef(BoneType.ForearmLeft, BoneType.UpperArmLeft, JointType.ElbowLeft, JointType.WristLeft),
            new BoneDef(BoneType.HandLeft, BoneType.ForearmLeft, JointType.WristLeft, JointType.HandLeft),
            new BoneDef(BoneType.HandTipLeft, BoneType.HandLeft, JointType.HandLeft, JointType.HandTipLeft),
            new BoneDef(BoneType.ThumbLeft, BoneType.HandLeft, JointType.HandLeft, JointType.ThumbLeft),
            
            // Right Leg
            new BoneDef(BoneType.HipRight, BoneType.Root, JointType.SpineBase, JointType.HipRight),
            new BoneDef(BoneType.ThighRight, BoneType.HipRight, JointType.HipRight, JointType.KneeRight),
            new BoneDef(BoneType.ShinRight, BoneType.ThighRight, JointType.KneeRight, JointType.AnkleRight),
            new BoneDef(BoneType.FootRight, BoneType.ShinRight, JointType.AnkleRight, JointType.FootRight),
            
            // Left Leg
            new BoneDef(BoneType.HipLeft, BoneType.Root, JointType.SpineBase, JointType.HipLeft),
            new BoneDef(BoneType.ThighLeft, BoneType.HipLeft, JointType.HipLeft, JointType.KneeLeft),
            new BoneDef(BoneType.ShinLeft, BoneType.ThighLeft, JointType.KneeLeft, JointType.AnkleLeft),
            new BoneDef(BoneType.FootLeft, BoneType.ShinLeft, JointType.AnkleLeft, JointType.FootLeft),
        };

        public static BoneDef FindBone(BoneType type)
        {
            return Array.Find(Bones, b => b.Type == type);
        }
    }
}

using Mokap.Schemas.RecorderMessages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mokap.Data
{
    enum BoneType
    {
        // Torso
        Head_Neck,
        Neck_SpineShoulder,
        SpineShoulder_SpineMid,
        SpineMid_SpineBase,
        SpineShoulder_ShoulderRight,
        SpineShoulder_ShoulderLeft,
        SpineBase_HipRight,
        SpineBase_HipLeft,

        // Right Arm
        ShoulderRight_ElbowRight,
        ElbowRight_WristRight,
        WristRight_HandRight,
        HandRight_HandTipRight,
        WristRight_ThumbRight,

        // Left Arm
        ShoulderLeft_ElbowLeft,
        ElbowLeft_WristLeft,
        WristLeft_HandLeft,
        HandLeft_HandTipLeft,
        WristLeft_ThumbLeft,

        // Right Leg
        HipRight_KneeRight,
        KneeRight_AnkleRight,
        AnkleRight_FootRight,

        // Left Leg
        HipLeft_KneeLeft,
        KneeLeft_AnkleLeft,
        AnkleLeft_FootLeft,
    }

    static class BoneTypeHelper
    {
        public static JointType FromJoint(this BoneType boneType)
        {
            return (JointType)Enum.Parse(typeof(JointType), boneType.ToString().Split('_')[0]);
        }

        public static JointType ToJoint(this BoneType boneType)
        {
            return (JointType)Enum.Parse(typeof(JointType), boneType.ToString().Split('_')[1]);
        }

        public static IEnumerable<BoneType> BoneTypes()
        {
            return Enum.GetValues(typeof(BoneType)).Cast<BoneType>();
        }
    }
}

using Microsoft.Kinect;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    class Skeleton
    {
        private Vector3D initialPosition;
        private Quaternion initialRotation;
        private List<Bone> bones = new List<Bone>();
        private BvhFrameLine frames = new BvhFrameLine();

        #region Initialization

        public Skeleton(IBodyAdapter body)
        {
            this.initialPosition = body.GetJointPosition(JointType.SpineBase);
            this.initialRotation = body.GetJointRotation(JointType.SpineBase);

            // Body
            var spineMidLength = GetBoneLength(body, JointType.SpineMid, JointType.SpineBase);
            var spineMid = CreateBone(JointType.SpineMid, null, spineMidLength);

            var spineShoulderLength = GetBoneLength(body, JointType.SpineMid, JointType.SpineShoulder);
            var spineShoulder = CreateBone(JointType.SpineShoulder, spineMid, spineShoulderLength);

            var neckLength = GetBoneLength(body, JointType.SpineShoulder, JointType.Neck);
            var neck = CreateBone(JointType.Neck, spineShoulder, neckLength);

            var headLength = GetBoneLength(body, JointType.Neck, JointType.Head);
            var head = CreateBone(JointType.Head, neck, headLength);

            // Arm
            var shoulderLength = GetBoneLength(body, JointType.SpineShoulder, JointType.ShoulderLeft, JointType.SpineShoulder, JointType.ShoulderRight);
            var shoulderLeft = CreateBone(JointType.ShoulderLeft, spineShoulder, shoulderLength);
            var shoulderRight = CreateBone(JointType.ShoulderRight, spineShoulder, shoulderLength);

            var elbowLength = GetBoneLength(body, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.ShoulderRight, JointType.ElbowRight);
            var elbowLeft = CreateBone(JointType.ElbowLeft, shoulderLeft, elbowLength);
            var elbowRight = CreateBone(JointType.ElbowRight, shoulderRight, elbowLength);

            var wristLength = GetBoneLength(body, JointType.ElbowLeft, JointType.WristLeft, JointType.ElbowRight, JointType.WristRight);
            var wristLeft = CreateBone(JointType.WristLeft, elbowLeft, wristLength);
            var wristRight = CreateBone(JointType.WristRight, elbowRight, wristLength);

            var handLength = GetBoneLength(body, JointType.WristLeft, JointType.HandLeft, JointType.WristRight, JointType.HandRight);
            var handLeft = CreateBone(JointType.HandLeft, wristLeft, handLength);
            var handRight = CreateBone(JointType.HandRight, wristRight, handLength);

            var handTipLength = GetBoneLength(body, JointType.HandLeft, JointType.HandTipLeft, JointType.HandRight, JointType.HandTipRight);
            var handTipLeft = CreateBone(JointType.HandTipLeft, handLeft, handTipLength);
            var handTipRight = CreateBone(JointType.HandTipRight, handRight, handTipLength);

            var thumbLength = GetBoneLength(body, JointType.HandLeft, JointType.ThumbLeft, JointType.HandRight, JointType.ThumbRight);
            var thumbLeft = CreateBone(JointType.ThumbLeft, handLeft, thumbLength);
            var thumbRight = CreateBone(JointType.ThumbRight, handRight, thumbLength);

            // Leg
            var hipLength = GetBoneLength(body, JointType.SpineBase, JointType.HipLeft, JointType.SpineBase, JointType.HipRight);
            var hipLeft = CreateBone(JointType.HipLeft, null, hipLength);
            var hipRight = CreateBone(JointType.HipRight, null, hipLength);

            var kneeLength = GetBoneLength(body, JointType.HipLeft, JointType.KneeLeft, JointType.HipRight, JointType.KneeRight);
            var kneeLeft = CreateBone(JointType.KneeLeft, hipLeft, kneeLength);
            var kneeRight = CreateBone(JointType.KneeRight, hipRight, kneeLength);

            var ankleLength = GetBoneLength(body, JointType.KneeLeft, JointType.AnkleLeft, JointType.KneeRight, JointType.AnkleRight);
            var ankleLeft = CreateBone(JointType.AnkleLeft, kneeLeft, ankleLength);
            var ankleRight = CreateBone(JointType.AnkleRight, kneeRight, ankleLength);

            var footLength = GetBoneLength(body, JointType.AnkleLeft, JointType.FootLeft, JointType.AnkleRight, JointType.FootRight);
            var footLeft = CreateBone(JointType.FootLeft, ankleLeft, footLength);
            var footRight = CreateBone(JointType.FootRight, ankleRight, footLength);
        }

        private Bone CreateBone(JointType name, Bone parent, double length)
        {
            var bone = new Bone(this, name, parent, length);

            this.bones.Add(bone);

            return bone;
        }

        private static double GetBoneLength(IBodyAdapter body, JointType from, JointType to)
        {
            var fromPos = body.GetJointPosition(from);
            var toPos = body.GetJointPosition(to);

            return (fromPos - toPos).Length;
        }

        private static double GetBoneLength(IBodyAdapter body, JointType from1, JointType to1, JointType from2, JointType to2)
        {
            var length1 = GetBoneLength(body, from1, to1);
            var length2 = GetBoneLength(body, from2, to2);

            return (length1 + length2) / 2;
        }

        #endregion

        public void AppendFrame(IBodyAdapter body)
        {
            var position = body.GetJointPosition(JointType.SpineBase);
            var rotation = body.GetJointRotation(JointType.SpineBase);
            this.frames.Add(new BvhFrame(position, rotation));

            foreach (var bone in Bones)
            {
                bone.AppendFrame(body);
            }
        }

        #region Properties

        public Vector3D InitialPosition
        {
            get { return this.initialPosition; }
        }

        public Quaternion InitialRotation
        {
            get { return this.initialRotation; }
        }

        public BvhFrameLine Frames
        {
            get { return this.frames; }
        }

        public IEnumerable<Bone> Bones
        {
            get { return this.bones; }
        }

        public IEnumerable<Bone> Children
        {
            get
            {
                return from bone in this.bones
                       where bone.Parent == null
                       select bone;
            }
        }

        public IEnumerable<Bone> Descendants
        {
            get
            {
                foreach (var child in Children)
                {
                    yield return child;

                    foreach (var descendant in child.Descendants)
                    {
                        yield return descendant;
                    }
                }
            }
        }

        #endregion
    }
}
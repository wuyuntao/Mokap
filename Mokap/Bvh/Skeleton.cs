using Mokap.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using JointType = Mokap.Schemas.RecorderMessages.JointType;

namespace Mokap.Bvh
{
    class Skeleton
    {
        private Vector3D initialPosition;
        private Quaternion initialRotation;
        private List<Bone> bones = new List<Bone>();
        private BvhFrameLine frames = new BvhFrameLine();

        #region Initialization

        public Skeleton(BodyFrameData.Body body)
        {
            initialPosition = body.Joints[JointType.SpineBase].Position3D;
            initialRotation = Quaternion.Identity;     // TODO: Is rotation of SpineBase necessary

            // directions
            var up = new Vector3D(0, 1, 0);
            var down = new Vector3D(0, -1, 0);
            var left = new Vector3D(-1, 0, 0);
            var right = new Vector3D(1, 0, 0);
            var forward = new Vector3D(0, 0, 1);
            var back = new Vector3D(0, 0, -1);

            // Body
            var spineMidLength = GetBoneLength(body, JointType.SpineMid, JointType.SpineBase);
            var spineMid = CreateBone(JointType.SpineMid, null, spineMidLength, up);

            var spineShoulderLength = GetBoneLength(body, JointType.SpineMid, JointType.SpineShoulder);
            var spineShoulder = CreateBone(JointType.SpineShoulder, spineMid, spineShoulderLength, up);

            var neckLength = GetBoneLength(body, JointType.SpineShoulder, JointType.Neck);
            var neck = CreateBone(JointType.Neck, spineShoulder, neckLength, up);

            var headLength = GetBoneLength(body, JointType.Neck, JointType.Head);
            var head = CreateBone(JointType.Head, neck, headLength, up);

            // Arm
            var shoulderLength = GetBoneLength(body, JointType.SpineShoulder, JointType.ShoulderLeft, JointType.SpineShoulder, JointType.ShoulderRight);
            var shoulderLeft = CreateBone(JointType.ShoulderLeft, spineShoulder, shoulderLength, left);
            var shoulderRight = CreateBone(JointType.ShoulderRight, spineShoulder, shoulderLength, right);

            var elbowLength = GetBoneLength(body, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.ShoulderRight, JointType.ElbowRight);
            var elbowLeft = CreateBone(JointType.ElbowLeft, shoulderLeft, elbowLength, left);
            var elbowRight = CreateBone(JointType.ElbowRight, shoulderRight, elbowLength, right);

            var wristLength = GetBoneLength(body, JointType.ElbowLeft, JointType.WristLeft, JointType.ElbowRight, JointType.WristRight);
            var wristLeft = CreateBone(JointType.WristLeft, elbowLeft, wristLength, left);
            var wristRight = CreateBone(JointType.WristRight, elbowRight, wristLength, right);

            var handLength = GetBoneLength(body, JointType.WristLeft, JointType.HandLeft, JointType.WristRight, JointType.HandRight);
            var handLeft = CreateBone(JointType.HandLeft, wristLeft, handLength, left);
            var handRight = CreateBone(JointType.HandRight, wristRight, handLength, right);

            var handTipLength = GetBoneLength(body, JointType.HandLeft, JointType.HandTipLeft, JointType.HandRight, JointType.HandTipRight);
            var handTipLeft = CreateBone(JointType.HandTipLeft, handLeft, handTipLength, left);
            var handTipRight = CreateBone(JointType.HandTipRight, handRight, handTipLength, right);

            var thumbLength = GetBoneLength(body, JointType.HandLeft, JointType.ThumbLeft, JointType.HandRight, JointType.ThumbRight);
            var thumbLeft = CreateBone(JointType.ThumbLeft, handLeft, thumbLength, forward);
            var thumbRight = CreateBone(JointType.ThumbRight, handRight, thumbLength, forward);

            // Leg
            var hipLength = GetBoneLength(body, JointType.SpineBase, JointType.HipLeft, JointType.SpineBase, JointType.HipRight);
            var hipLeft = CreateBone(JointType.HipLeft, null, hipLength, left);
            var hipRight = CreateBone(JointType.HipRight, null, hipLength, right);

            var kneeLength = GetBoneLength(body, JointType.HipLeft, JointType.KneeLeft, JointType.HipRight, JointType.KneeRight);
            var kneeLeft = CreateBone(JointType.KneeLeft, hipLeft, kneeLength, down);
            var kneeRight = CreateBone(JointType.KneeRight, hipRight, kneeLength, down);

            var ankleLength = GetBoneLength(body, JointType.KneeLeft, JointType.AnkleLeft, JointType.KneeRight, JointType.AnkleRight);
            var ankleLeft = CreateBone(JointType.AnkleLeft, kneeLeft, ankleLength, down);
            var ankleRight = CreateBone(JointType.AnkleRight, kneeRight, ankleLength, down);

            var footLength = GetBoneLength(body, JointType.AnkleLeft, JointType.FootLeft, JointType.AnkleRight, JointType.FootRight);
            var footLeft = CreateBone(JointType.FootLeft, ankleLeft, footLength, forward);
            var footRight = CreateBone(JointType.FootRight, ankleRight, footLength, forward);
        }

        private Bone CreateBone(JointType name, Bone parent, double length, Vector3D tPoseDirection)
        {
            var bone = new Bone(this, name, parent, length, tPoseDirection);

            bones.Add(bone);

            return bone;
        }

        private static double GetBoneLength(BodyFrameData.Body body, JointType from, JointType to)
        {
            var fromPos = body.Joints[from].Position3D;
            var toPos = body.Joints[to].Position3D;

            return (fromPos - toPos).Length;
        }

        private static double GetBoneLength(BodyFrameData.Body body, JointType from1, JointType to1, JointType from2, JointType to2)
        {
            var length1 = GetBoneLength(body, from1, to1);
            var length2 = GetBoneLength(body, from2, to2);

            return (length1 + length2) / 2;
        }

        #endregion

        public void AppendFrame(BodyFrameData.Body body)
        {
            var position = body.Joints[JointType.SpineBase].Position3D;
            var rotation = body.Joints[JointType.SpineBase].Rotation;
            //var rotation = Quaternion.Identity;

            frames.Add(new BvhFrame(position, rotation));

            foreach (var bone in Bones)
            {
                bone.AppendFrame(body);
            }
        }

        #region Properties

        public Vector3D InitialPosition
        {
            get { return initialPosition; }
        }

        public Quaternion InitialRotation
        {
            get { return initialRotation; }
        }

        public BvhFrameLine Frames
        {
            get { return frames; }
        }

        public IEnumerable<Bone> Bones
        {
            get { return bones; }
        }

        public IEnumerable<Bone> Children
        {
            get
            {
                return from bone in bones
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
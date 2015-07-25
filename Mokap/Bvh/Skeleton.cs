using Microsoft.Kinect;
using NLog;

namespace Mokap.Bvh
{
    class Skeleton
    {
        private Joint root;

        Skeleton(Joint root)
        {
            this.root = root;
        }

        /// <summary>
        /// Factory method to create Skeleton from Kinect Body
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Skeleton Create(Body body)
        {
            // Body
            var spineBase = Joint.CreateRoot(body.Joints[JointType.SpineBase].Position.ToVector3D());
            var spineMid = CreateChildJoint(body, spineBase, JointType.SpineMid);
            var spineShoulder = CreateChildJoint(body, spineMid, JointType.SpineShoulder);
            var neck = CreateChildJoint(body, spineShoulder, JointType.Neck);
            var head = CreateChildJoint(body, neck, JointType.Head);

            // Left leg
            var hipLeft = CreateChildJoint(body, spineBase, JointType.HipLeft);
            var kneeLeft = CreateChildJoint(body, hipLeft, JointType.KneeLeft);
            var angleLeft = CreateChildJoint(body, kneeLeft, JointType.KneeLeft);
            var footLeft = CreateChildJoint(body, angleLeft, JointType.FootLeft);

            // Right leg
            var hipRight = CreateChildJoint(body, spineBase, JointType.HipRight);
            var kneeRight = CreateChildJoint(body, hipRight, JointType.KneeRight);
            var angleRight = CreateChildJoint(body, kneeRight, JointType.KneeRight);
            var footRight = CreateChildJoint(body, angleRight, JointType.FootRight);

            // Left arm
            var shoulderLeft = CreateChildJoint(body, spineShoulder, JointType.ShoulderLeft);
            var elbowLeft = CreateChildJoint(body, shoulderLeft, JointType.ElbowLeft);
            var handLeft = CreateChildJoint(body, elbowLeft, JointType.HandLeft);
            var handTipLeft = CreateChildJoint(body, handLeft, JointType.HandTipLeft);
            var thumbLeft = CreateChildJoint(body, handLeft, JointType.ThumbLeft);

            // Right arm
            var shoulderRight = CreateChildJoint(body, spineShoulder, JointType.ShoulderRight);
            var elbowRight = CreateChildJoint(body, shoulderRight, JointType.ElbowRight);
            var handRight = CreateChildJoint(body, elbowRight, JointType.HandRight);
            var handTipRight = CreateChildJoint(body, handRight, JointType.HandTipRight);
            var thumbRight = CreateChildJoint(body, handRight, JointType.ThumbRight);

            return new Skeleton(spineBase);
        }

        static Joint CreateChildJoint(Body body, Joint parent, JointType jointType)
        {
            return parent.CreateChild(jointType, body.Joints[jointType].Position.ToVector3D());
        }
    }
}
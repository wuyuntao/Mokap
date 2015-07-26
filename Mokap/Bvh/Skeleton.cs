using Microsoft.Kinect;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

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
            var spineBase = CreateRootJoint(body);
            var spineMid = CreateChildJoint(body, spineBase, JointType.SpineMid);
            var spineShoulder = CreateChildJoint(body, spineMid, JointType.SpineShoulder);
            var neck = CreateChildJoint(body, spineShoulder, JointType.Neck);
            var head = CreateChildJoint(body, neck, JointType.Head);

            // Left leg
            var hipLeft = CreateChildJoint(body, spineBase, JointType.HipLeft);
            var kneeLeft = CreateChildJoint(body, hipLeft, JointType.KneeLeft);
            var angleLeft = CreateChildJoint(body, kneeLeft, JointType.AnkleLeft);
            var footLeft = CreateChildJoint(body, angleLeft, JointType.FootLeft);

            // Right leg
            var hipRight = CreateChildJoint(body, spineBase, JointType.HipRight);
            var kneeRight = CreateChildJoint(body, hipRight, JointType.KneeRight);
            var angleRight = CreateChildJoint(body, kneeRight, JointType.AnkleRight);
            var footRight = CreateChildJoint(body, angleRight, JointType.FootRight);

            // Left arm
            var shoulderLeft = CreateChildJoint(body, spineShoulder, JointType.ShoulderLeft);
            var elbowLeft = CreateChildJoint(body, shoulderLeft, JointType.ElbowLeft);
            var wristLeft = CreateChildJoint(body, elbowLeft, JointType.WristLeft);
            var handLeft = CreateChildJoint(body, wristLeft, JointType.HandLeft);
            var handTipLeft = CreateChildJoint(body, handLeft, JointType.HandTipLeft);
            var thumbLeft = CreateChildJoint(body, handLeft, JointType.ThumbLeft);

            // Right arm
            var shoulderRight = CreateChildJoint(body, spineShoulder, JointType.ShoulderRight);
            var elbowRight = CreateChildJoint(body, shoulderRight, JointType.ElbowRight);
            var wristRight = CreateChildJoint(body, elbowRight, JointType.WristRight);
            var handRight = CreateChildJoint(body, wristRight, JointType.HandRight);
            var handTipRight = CreateChildJoint(body, handRight, JointType.HandTipRight);
            var thumbRight = CreateChildJoint(body, handRight, JointType.ThumbRight);

            return new Skeleton(spineBase);
        }

        private static Joint CreateRootJoint(Body body)
        {
            var position = body.Joints[JointType.SpineBase].Position.ToVector3D();
            var rotation = body.JointOrientations[JointType.SpineBase].Orientation.ToQuaternion();

            return Joint.CreateRoot(position, rotation);
        }

        private static Joint CreateChildJoint(Body body, Joint parent, JointType jointType)
        {
            var position = body.Joints[jointType].Position.ToVector3D();
            var rotation = body.JointOrientations[jointType].Orientation.ToQuaternion();

            return parent.CreateChild(jointType, position, rotation);
        }

        public Frame CreateFrame(Body body)
        {
            var rotations = new List<Vector3D>();

            foreach (var joint in Joints)
            {
                var position = body.Joints[joint.Type].Position.ToVector3D();
                var rotation = body.JointOrientations[joint.Type].Orientation.ToQuaternion();

                joint.Update(position, rotation);

                rotations.Add(joint.Rotation.ToEularAngles());
            }

            return new Frame(this.root.Position, rotations);
        }

        public Joint Root
        {
            get { return this.root; }
        }

        public IEnumerable<Joint> Joints
        {
            get
            {
                yield return this.root;

                foreach (var joint in this.root.Descendants)
                {
                    yield return joint;
                }
            }
        }
    }
}
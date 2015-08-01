using Microsoft.Kinect;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    class Bone
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private JointType name;
        private Skeleton skeleton;
        private Bone parent;
        private double length;
        private Vector3D tPoseDirection;

        private BvhFrameLine frames = new BvhFrameLine();

        public Bone(Skeleton skeleton, JointType name, Bone parent, double length, Vector3D direction)
        {
            this.skeleton = skeleton;
            this.name = name;
            this.parent = parent;
            this.length = length;
            this.tPoseDirection = direction;

            logger.Trace("Create {0}. Length: {1}", this, this.length);
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}>", base.ToString(), this.name);
        }

        public void AppendFrame(IBodyAdapter body)
        {
            var position = body.GetJointPosition(this.name);
            var parentName = GetParentName(this);
            var parentPosition = body.GetJointPosition(parentName);

            Quaternion rotation;
            if (this.parent != null)
            {
                var ancestorName = GetParentName(this.parent);
                var ancestorPosition = body.GetJointPosition(ancestorName);

                var parentDirection = parentPosition - ancestorPosition;
                var direction = position - parentPosition;

                rotation = KinectHelper.LookRotation(parentDirection, this.parent.tPoseDirection);
                rotation.Invert();
                rotation = rotation * KinectHelper.LookRotation(direction, this.tPoseDirection);

                logger.Trace("{0} ({1}) -> {2} ({3}) -> {4} ({5}) : {6} -> {7} : {8} / {9}"
                        , ancestorName, ancestorPosition
                        , parentName, parentPosition
                        , this.name, position
                        , parentDirection, direction
                        , rotation, KinectHelper.ToEularAngle(rotation));
            }
            else
            {
                var direction = position - parentPosition;
                rotation = KinectHelper.LookRotation(direction, this.tPoseDirection);

                logger.Trace("{0} ({1}) -> {2} ({3}) : {4} : {5} / {6}"
                        , parentName, parentPosition
                        , this.name, position
                        , direction
                        , rotation, KinectHelper.ToEularAngle(rotation));
            }

            this.frames.Add(new BvhFrame(rotation));
        }

        private JointType GetParentName(Bone bone)
        {
            return bone.parent == null ? JointType.SpineBase : bone.parent.name;
        }

        #region Properties

        public JointType Name
        {
            get { return this.name; }
        }

        public Vector3D InitialOffset
        {
            get
            {
                switch (this.name)
                {
                    // Up
                    case JointType.Head:
                    case JointType.Neck:
                    case JointType.SpineShoulder:
                    case JointType.SpineMid:
                    case JointType.SpineBase:
                        return new Vector3D(0, 1, 0) * this.length;

                    // Down
                    case JointType.KneeLeft:
                    case JointType.KneeRight:
                    case JointType.AnkleLeft:
                    case JointType.AnkleRight:
                        return new Vector3D(0, -1, 0) * this.length;

                    // Left
                    case JointType.ShoulderLeft:
                    case JointType.ElbowLeft:
                    case JointType.WristLeft:
                    case JointType.HandLeft:
                    case JointType.HandTipLeft:
                    case JointType.HipLeft:
                        return new Vector3D(-1, 0, 0) * this.length;

                    // Right
                    case JointType.ShoulderRight:
                    case JointType.ElbowRight:
                    case JointType.WristRight:
                    case JointType.HandRight:
                    case JointType.HandTipRight:
                    case JointType.HipRight:
                        return new Vector3D(1, 0, 0) * this.length;

                    // Forward
                    case JointType.ThumbLeft:
                    case JointType.ThumbRight:
                    case JointType.FootLeft:
                    case JointType.FootRight:
                        return new Vector3D(0, 0, 1) * this.length;

                    default:
                        throw new NotSupportedException(this.name.ToString());
                }

            }
        }

        public Bone Parent
        {
            get { return this.parent; }
        }

        public IEnumerable<Bone> Children
        {
            get
            {
                return from bone in this.skeleton.Bones
                       where bone.parent == this
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

        public double Length
        {
            get { return this.length; }
        }

        public BvhFrameLine Frames
        {
            get { return this.frames; }
        }

        public bool IsEnd
        {
            get { return !Children.Any(); }
        }

        #endregion
    }
}
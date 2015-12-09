using Mokap.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using JointType = Mokap.Schemas.RecorderMessages.JointType;

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
            tPoseDirection = direction;

            logger.Trace("Create {0}. Length: {1}", this, this.length);
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}>", base.ToString(), name);
        }

        public void AppendFrame(BodyFrameData.Body body)
        {
            var position = body.Joints[name].Position3D;
            var parentName = GetParentName(this);
            var parentPosition = body.Joints[parentName].Position3D;

            Quaternion rotation;
            if (parent != null)
            {
                var ancestorName = GetParentName(parent);
                var ancestorPosition = body.Joints[ancestorName].Position3D;

                var parentDirection = parentPosition - ancestorPosition;
                var direction = position - parentPosition;

                rotation = KinectHelper.LookRotation(parentDirection, parent.tPoseDirection);
                rotation.Invert();
                rotation = rotation * KinectHelper.LookRotation(direction, tPoseDirection);

                logger.Trace("{0} ({1}) -> {2} ({3}) -> {4} ({5}) : {6} -> {7} : {8} / {9}"
                        , ancestorName, ancestorPosition
                        , parentName, parentPosition
                        , name, position
                        , parentDirection, direction
                        , rotation, KinectHelper.ToEularAngle(rotation));
            }
            else
            {
                var direction = position - parentPosition;
                rotation = KinectHelper.LookRotation(direction, tPoseDirection);

                logger.Trace("{0} ({1}) -> {2} ({3}) : {4} : {5} / {6}"
                        , parentName, parentPosition
                        , name, position
                        , direction
                        , rotation, KinectHelper.ToEularAngle(rotation));
            }

            frames.Add(new BvhFrame(rotation));
        }

        private JointType GetParentName(Bone bone)
        {
            return bone.parent == null ? JointType.SpineBase : bone.parent.name;
        }

        #region Properties

        public JointType Name
        {
            get { return name; }
        }

        public Vector3D InitialOffset
        {
            get
            {
                switch (name)
                {
                    // Up
                    case JointType.Head:
                    case JointType.Neck:
                    case JointType.SpineShoulder:
                    case JointType.SpineMid:
                    case JointType.SpineBase:
                        return new Vector3D(0, 1, 0) * length;

                    // Down
                    case JointType.KneeLeft:
                    case JointType.KneeRight:
                    case JointType.AnkleLeft:
                    case JointType.AnkleRight:
                        return new Vector3D(0, -1, 0) * length;

                    // Left
                    case JointType.ShoulderLeft:
                    case JointType.ElbowLeft:
                    case JointType.WristLeft:
                    case JointType.HandLeft:
                    case JointType.HandTipLeft:
                    case JointType.HipLeft:
                        return new Vector3D(-1, 0, 0) * length;

                    // Right
                    case JointType.ShoulderRight:
                    case JointType.ElbowRight:
                    case JointType.WristRight:
                    case JointType.HandRight:
                    case JointType.HandTipRight:
                    case JointType.HipRight:
                        return new Vector3D(1, 0, 0) * length;

                    // Forward
                    case JointType.ThumbLeft:
                    case JointType.ThumbRight:
                    case JointType.FootLeft:
                    case JointType.FootRight:
                        return new Vector3D(0, 0, 1) * length;

                    default:
                        throw new NotSupportedException(name.ToString());
                }

            }
        }

        public Bone Parent
        {
            get { return parent; }
        }

        public IEnumerable<Bone> Children
        {
            get
            {
                return from bone in skeleton.Bones
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
            get { return length; }
        }

        public BvhFrameLine Frames
        {
            get { return frames; }
        }

        public bool IsEnd
        {
            get { return !Children.Any(); }
        }

        #endregion
    }
}
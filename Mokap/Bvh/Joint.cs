using Microsoft.Kinect;
using Mokap.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    class Joint
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private JointType type;
        private Joint parent;
        private List<Joint> children;
        private double length;
        private Vector3D position;
        private Quaternion rotation;

        Joint(Joint parent, JointType type, Vector3D position, Quaternion rotation)
        {
            this.parent = parent;
            this.type = type;

            this.length = parent == null
                ? Settings.Default.SpineBaseOffsetY
                : (position - parent.position).Length;
            this.position = position;
            this.rotation = rotation;

            logger.Trace("Create {0}. Length: {1}, Position: {2}, Rotation: {3}", this, this.length, this.position, this.rotation);
        }

        public static Joint CreateRoot(Vector3D position, Quaternion rotation)
        {
            return new Joint(null, JointType.SpineBase, position, rotation);
        }

        public override string ToString()
        {
            return string.Format("{0}<Type: {1}>", base.ToString(), this.type);
        }

        public Joint CreateChild(JointType type, Vector3D position, Quaternion rotation)
        {
            var joint = new Joint(this, type, position, rotation);

            if (this.children == null)
            {
                this.children = new List<Joint>();
            }

            this.children.Add(joint);

            return joint;
        }

        public void Update(Vector3D position, Quaternion rotation)
        {
            var length = parent == null
                ? Settings.Default.SpineBaseOffsetY
                : (position - parent.position).Length;

            if (length > this.length)
                this.length = length;

            this.position = position;
            this.rotation = rotation;

            logger.Trace("Update {0}. Length: {1}, Position: {2}, Rotation: {3}", this, this.length, this.position, this.rotation);
        }

        #region Properties

        public Joint Parent
        {
            get { return this.parent; }
        }

        public JointType Type
        {
            get { return this.type; }
        }

        public IEnumerable<Joint> Children
        {
            get { return this.children; }
        }

        public IEnumerable<Joint> Descendants
        {
            get
            {
                if (this.children != null)
                {
                    foreach (var child in this.children)
                    {
                        yield return child;

                        foreach (var descendant in child.Descendants)
                        {
                            yield return descendant;
                        }
                    }
                }
            }
        }

        public double Length
        {
            get { return this.length; }
        }

        public Vector3D Position
        {
            get { return this.position; }
        }

        public Vector3D Offset
        {
            get
            {
                if (this.parent == null)
                    return this.position;
                else
                    return this.position - this.parent.position;
            }
        }

        public Quaternion Rotation
        {
            get { return this.rotation; }
        }

        public bool IsRoot
        {
            get { return this.parent == null; }
        }

        public bool IsEnd
        {
            get { return this.children == null; }
        }
        
        #endregion
    }
}
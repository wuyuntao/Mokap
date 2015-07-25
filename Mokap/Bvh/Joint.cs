using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    class Joint
    {
        private JointType type;
        private Joint parent;
        private List<Joint> children;
        private Vector3D position;

        Joint(Joint parent, JointType type, Vector3D position)
        {
            this.parent = parent;
            this.type = type;
            this.position = position;
        }

        public static Joint CreateRoot(Vector3D position)
        {
            return new Joint(null, JointType.SpineBase, position);
        }

        public override string ToString()
        {
            return string.Format("{0}<Parent: {1}, Type: {2}>", base.ToString(), this.parent, this.type);
        }

        public Joint CreateChild(JointType type, Vector3D position)
        {
            var joint = new Joint(this, type, position);

            if (this.children == null)
            {
                this.children = new List<Joint>();
            }

            this.children.Add(joint);

            return joint;
        }

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

        public bool IsRoot
        {
            get { return this.parent == null; }
        }

        public bool IsEnd
        {
            get { return this.children == null; }
        }
    }
}
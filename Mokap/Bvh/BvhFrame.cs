using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    class BvhFrame
    {
        private Vector3D offset;

        private Quaternion rotation;

        internal BvhFrame(Vector3D positon, Quaternion rotation)
        {
            offset = positon;
            this.rotation = rotation;
        }

        internal BvhFrame(Quaternion rotation)
            : this(new Vector3D(), rotation)
        { }

        public Vector3D Offset
        {
            get { return offset; }
        }

        public Vector3D Rotation
        {
            get { return KinectHelper.ToEularAngle(rotation); }
        }
    }
}
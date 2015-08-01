using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace Mokap
{
    class KinectBodyAdapter : IBodyAdapter
    {
        private Body body;

        public KinectBodyAdapter(Body body)
        {
            this.body = body;
        }

        Vector3D IBodyAdapter.GetJointPosition(JointType type)
        {
            var p = this.body.Joints[type].Position;

            return new Vector3D(p.X, p.Y, p.Z);
        }

        Quaternion IBodyAdapter.GetJointRotation(JointType type)
        {
            var o = this.body.JointOrientations[type].Orientation;

            return new Quaternion(o.X, o.Y, o.Z, o.W);
        }

        TrackingState IBodyAdapter.GetJointState(JointType type)
        {
            return this.body.Joints[type].TrackingState;
        }
    }
}

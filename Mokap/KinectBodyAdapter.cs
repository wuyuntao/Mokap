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
            return this.body.Joints[type].Position.ToVector3D();
        }

        Quaternion IBodyAdapter.GetJointRotation(JointType type)
        {
            return this.body.JointOrientations[type].Orientation.ToQuaternion();
        }

        TrackingState IBodyAdapter.GetJointState(JointType type)
        {
            return this.body.Joints[type].TrackingState;
        }
    }
}

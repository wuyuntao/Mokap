using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace Mokap
{
    interface IBodyAdapter
    {
        Vector3D GetJointPosition(JointType type);
        Quaternion GetJointRotation(JointType type);

        TrackingState GetJointState(JointType type);
    }
}
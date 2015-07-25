using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace Mokap
{
    static class KinectHelper
    {
        public static Vector3D ToVector3D(this CameraSpacePoint point)
        {
            return new Vector3D(point.X, point.Y, point.Z);
        }
    }
}

using Microsoft.Kinect;
using System;
using System.Windows.Media.Media3D;

namespace Mokap
{
    static class KinectHelper
    {
        public static Vector3D ToVector3D(this CameraSpacePoint point)
        {
            return new Vector3D(point.X, point.Y, point.Z);
        }

        public static Quaternion ToQuaternion(this Vector4 vector4)
        {
            return new Quaternion(vector4.X, vector4.Y, vector4.Z, vector4.W);
        }

        public static Vector3D ToEularAngles(this Quaternion quaternion)
        {
            var rotation = new Vector3D();
            rotation.X = Math.Asin(2 * (quaternion.W * quaternion.Y - quaternion.Z * quaternion.X));
            var test = quaternion.X * quaternion.Y + quaternion.Z * quaternion.W;
            if (test == 0.5)
            {
                rotation.Y = 2 * Math.Atan2(quaternion.X, quaternion.W);
                rotation.Z = 0;
            }
            else if (test == -0.5)
            {
                rotation.Y = -2 * Math.Atan2(quaternion.X, quaternion.W);
                rotation.Z = 0;
            }
            else
            {
                rotation.Y = Math.Atan(2 * (quaternion.W * quaternion.Z + quaternion.Y * quaternion.Y) / (1 - 2 * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z)));
                rotation.Z = Math.Atan(2 * (quaternion.W * quaternion.X + quaternion.Y * quaternion.Z) / (1 - 2 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y)));
            }

            return rotation;
        }
    }
}

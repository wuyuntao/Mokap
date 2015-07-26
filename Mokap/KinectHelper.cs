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

        public static Vector3D ToEular(this Quaternion quaternion)
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

        public static Vector3D ToEularAngles(this Quaternion quaternion)
        {
            var rotation = quaternion.ToEular();

            return new Vector3D()
            {
                X = RadToDeg(rotation.X),
                Y = RadToDeg(rotation.Y),
                Z = RadToDeg(rotation.Z),
            };
        }

        public static double RadToDeg(double rad)
        {
            return rad / Math.PI * 180;
        }

        public static Vector3D GetTPoseDirection(JointType type)
        {
            switch (type)
            {
                // Up
                case JointType.Head:
                case JointType.Neck:
                case JointType.SpineShoulder:
                case JointType.SpineMid:
                case JointType.SpineBase:
                    return new Vector3D(0, 1, 0);

                // Down
                case JointType.KneeLeft:
                case JointType.KneeRight:
                case JointType.AnkleLeft:
                case JointType.AnkleRight:
                    return new Vector3D(0, -1, 0);

                // Left
                case JointType.ShoulderLeft:
                case JointType.ElbowLeft:
                case JointType.WristLeft:
                case JointType.HandLeft:
                case JointType.HandTipLeft:
                case JointType.HipLeft:
                    return new Vector3D(-1, 0, 0);

                // Right
                case JointType.ShoulderRight:
                case JointType.ElbowRight:
                case JointType.WristRight:
                case JointType.HandRight:
                case JointType.HandTipRight:
                case JointType.HipRight:
                    return new Vector3D(1, 0, 0);

                // Forward
                case JointType.ThumbLeft:
                case JointType.ThumbRight:
                case JointType.FootLeft:
                case JointType.FootRight:
                    return new Vector3D(0, 0, 1);

                default:
                    throw new NotSupportedException(type.ToString());
            }
        }
    }
}
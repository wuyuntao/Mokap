﻿using System;
using System.Diagnostics;
using System.Windows.Media.Media3D;

namespace Mokap
{
    static class QuaternionHelper
    {
        public static Quaternion Copy(this Quaternion q)
        {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public static Vector3D ToEulerAngles(this Quaternion q)
        {
            var rad2Deg = 180 / Math.PI;
            var yaw = rad2Deg * Math.Asin(Clamp(2 * (q.W * q.X - q.Y * q.Z), -1.0f, 1.0f));
            var pitch = rad2Deg * Math.Atan2(2 * (q.W * q.Y + q.Z * q.X), 1 - 2 * (q.X * q.X + q.Y * q.Y));
            var roll = rad2Deg * Math.Atan2(2 * (q.W * q.Z + q.X * q.Y), 1 - 2 * (q.Z * q.Z + q.X * q.X));

            yaw = yaw < 0 ? 360.0f + yaw : yaw;
            pitch = pitch < 0 ? 360.0f + pitch : pitch;
            roll = roll < 0 ? 360.0f + roll : roll;

            return new Vector3D(yaw, pitch, roll);
        }

        private static double Clamp(double value, double minValue, double maxValue)
        {
            if (value < minValue)
                return minValue;

            if (value > maxValue)
                return maxValue;

            return value;
        }

        public static Quaternion LookRotation(Vector3D forward, Vector3D upward)
        {
            Debug.Assert(forward.LengthSquared > 0.0f);

            return FromToRotation(upward, forward);
        }

        private static Quaternion FromToRotation(Vector3D from, Vector3D to)
        {
            from.Normalize();
            to.Normalize();

            var d = Vector3D.DotProduct(from, to);

            if (d >= 1.0f)
            {
                // In the case where the two vectors are pointing in the same
                // direction, we simply return the identity rotation.
                return Quaternion.Identity;
            }
            else if (d <= -1.0f)
            {
                // If the two vectors are pointing in opposite directions then we
                // need to supply a quaternion corresponding to a rotation of
                // PI-radians about an axis orthogonal to the fromDirection.
                var axis = Vector3D.CrossProduct(from, new Vector3D(1, 0, 0));
                if (axis.LengthSquared < 1e-6)
                {
                    // Bad luck. The x-axis and fromDirection are linearly
                    // dependent (colinear). We'll take the axis as the vector
                    // orthogonal to both the y-axis and fromDirection instead.
                    // The y-axis and fromDirection will clearly not be linearly
                    // dependent.
                    axis = Vector3D.CrossProduct(from, new Vector3D(0, 1, 0));
                }

                // Note that we need to normalize the axis as the cross product of
                // two unit vectors is not nececessarily a unit vector.
                axis.Normalize();
                return new Quaternion(axis, 180);
            }
            else
            {
                // Scalar component.
                var s = Math.Sqrt(from.LengthSquared * to.LengthSquared) + Vector3D.DotProduct(from, to);

                // Vector component.
                var v = Vector3D.CrossProduct(from, to);

                // Return the normalized quaternion rotation.
                var rotation = new Quaternion(v.X, v.Y, v.Z, s);
                rotation.Normalize();

                return rotation;
            }
        }

        public static string ToString(this Quaternion q, string format)
        {
            var template = string.Format(@"({{0:{0}}}, {{1:{0}}}, {{2:{0}}}, {{3:{0}}})", format);

            return string.Format(template, q.X, q.Y, q.Z, q.W);
        }

        public static string ToString(this Vector3D q, string format)
        {
            var template = string.Format(@"({{0:{0}}}, {{1:{0}}}, {{2:{0}}})", format);

            return string.Format(template, q.X, q.Y, q.Z);
        }
    }
}
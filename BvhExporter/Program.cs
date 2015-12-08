using BvhExporter.Data;
using GlmSharp;
using System;
using System.Text;

namespace BvhExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            //SimpleTest();
            SpineTest();

            Console.ReadKey();
        }

        private static void SimpleTest()
        {
            var motion = MotionSet.Simple1;

            foreach (var frame in motion.Frames)
            {
                if (frame.Time == 1)
                    continue;

                foreach (var boneDef in motion.Bones)
                {
                    Console.WriteLine("Frame {0} bone {1}", frame.Time, boneDef.Name);

                    var trans = mat4.Translate(boneDef.HeadPos);
                    var itrans = trans.Inverse;

                    //Console.WriteLine("trans");
                    //PrintMat4(trans);

                    //Console.WriteLine("itrans");
                    //PrintMat4(itrans);

                    var boneFra = frame.Bones.Find(b => b.Name == boneDef.Name);

                    var matDef = boneDef.Rotation.ToMat4;
                    var matFra = boneFra.Rotation.ToMat4;

                    Console.WriteLine("matDef. quat: {0}", boneDef.Rotation);
                    PrintMat4(matDef);

                    Console.WriteLine("matFra. quat: {0}", boneFra.Rotation);
                    PrintMat4(matFra);

                    mat4 matFin;
                    if (boneDef.ParentName != null)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        matFin = matFra * matDef.Inverse;
                        matFin = itrans * matFin * trans;
                    }

                    Console.WriteLine("matFin");
                    PrintMat4(matFin);

                    var q = quat.FromMat4(matFin);
                    Console.WriteLine("quat {0} vs {1}", q, boneFra.Rotation);
                    var angles = quat.FromMat4(matFin).EulerAngles;
                    Console.WriteLine("angles {0} vs {1}", angles / Math.PI * 180, boneFra.RotationAngles);
                }
            }
        }

        private static void SpineTest()
        {
            var motion = MotionSet.Spine;

            foreach (var frame in motion.Frames)
            {
                Console.WriteLine("==================================");
                Console.WriteLine("Frame {0}", frame.Time);
                foreach (var fBone in frame.Bones)
                {
                    var lQuat = GetAbsoluteRotation(fBone.Name, motion, frame, false);
                    if (fBone.ParentName != null)
                    {
                        var pQuat = GetAbsoluteRotation(fBone.ParentName, motion, frame, false);
                        //lQuat = pQuat.Inverse * lQuat;
                        // TODO dot product order?
                        lQuat = lQuat * pQuat.Inverse;
                    }

                    var lAngles = lQuat.EulerAngles / Math.PI * 180;

                    Console.WriteLine("{0} [FINAL] q: ({1}), angles: ({2})", fBone.Name, lQuat, lAngles);
                }
            }
        }

        private static quat GetAbsoluteRotation(string boneName, Motion motion, Frame frame, bool printLog)
        {
            var dBone = motion.Bones.Find(b => b.Name == boneName);
            var fBone = frame.Bones.Find(b => b.Name == boneName);

            var dLen = (ConvertToZUp(dBone.HeadPos) - ConvertToZUp(dBone.TailPos)).Length;
            var fLen = (ConvertToZUp(fBone.HeadPos) - ConvertToZUp(fBone.TailPos)).Length;

            var dDir = ConvertToZUp(dBone.TailPos) - ConvertToZUp(dBone.HeadPos);
            var fDir = ConvertToZUp(fBone.TailPos) - ConvertToZUp(fBone.HeadPos);

            var q = FromToRotation(dDir, fDir);
            var angles = q.EulerAngles / Math.PI * 180;

            if (printLog)
            {
                Console.WriteLine("{0} len: {1} -> {2}", fBone.Name, dLen, fLen);
                Console.WriteLine("{0} dir: ({1}) -> ({2})", fBone.Name, dDir, fDir);
                Console.WriteLine("{0} q: ({1}), angles: ({2})", fBone.Name, q, angles);
            }

            return q;
        }

        private static vec3 ConvertToZUp(vec3 vec)
        {
            //return vec;
            return new vec3(vec.x, -vec.z, vec.y);
        }

        private static quat FromToRotation(vec3 from, vec3 to)
        {
            from = from.Normalized;
            to = to.Normalized;

            var d = vec3.Dot(from, to);

            if (d >= 1.0f)
            {
                // In the case where the two vectors are pointing in the same
                // direction, we simply return the identity rotation.
                return quat.Identity;
            }
            else if (d <= -1.0f)
            {
                // If the two vectors are pointing in opposite directions then we
                // need to supply a quaternion corresponding to a rotation of
                // PI-radians about an axis orthogonal to the fromDirection.
                var axis = vec3.Cross(from, new vec3(1, 0, 0));
                if (axis.LengthSqr < 1e-6)
                {
                    // Bad luck. The x-axis and fromDirection are linearly
                    // dependent (colinear). We'll take the axis as the vector
                    // orthogonal to both the y-axis and fromDirection instead.
                    // The y-axis and fromDirection will clearly not be linearly
                    // dependent.
                    axis = vec3.Cross(from, new vec3(0, 1, 0));
                }

                // Note that we need to normalize the axis as the cross product of
                // two unit vectors is not nececessarily a unit vector.
                axis = axis.Normalized;
                return quat.FromAxisAngle((float)Math.PI, axis);
            }
            else
            {
                // Scalar component.
                var s = Math.Sqrt(from.LengthSqr * to.LengthSqr) + vec3.Dot(from, to);

                // Vector component.
                var v = vec3.Cross(from, to);

                // Return the normalized quaternion rotation.
                var rotation = new quat(v.x, v.y, v.z, (float)s);
                rotation = rotation.Normalized;

                return rotation;
            }
        }

        static void PrintMat4(mat4 mat4)
        {
            var builder = new StringBuilder();
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    builder.AppendFormat("{0:f4} ", mat4[col, row]);
                }

                builder.AppendLine();
            }

            Console.WriteLine(builder.ToString());
        }
    }
}

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
            //SpineTest();
            //HalfSpineTest();
            HalfSpine5Test();

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

                    var trans = dmat4.Translate(boneDef.HeadPos);
                    var itrans = trans.Inverse;

                    //Console.WriteLine("trans");
                    //PrintMat4(trans);

                    //Console.WriteLine("itrans");
                    //PrintMat4(itrans);

                    var boneFra = frame.Bones.Find(b => b.Name == boneDef.Name);

                    var matDef = boneDef.Rotation.ToMat4;
                    var matFra = boneFra.Rotation.ToMat4;

                    Console.WriteLine("matDef. dquat: {0}", boneDef.Rotation);
                    PrintMat4(matDef);

                    Console.WriteLine("matFra. dquat: {0}", boneFra.Rotation);
                    PrintMat4(matFra);

                    dmat4 matFin;
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

                    var q = dquat.FromMat4(matFin);
                    Console.WriteLine("dquat {0} vs {1}", q, boneFra.Rotation);
                    var angles = dquat.FromMat4(matFin).EulerAngles;
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
                    var lQuat = GetAbsoluteRotation(fBone.Name, motion, frame, true);
                    if (fBone.ParentName != null)
                    {
                        var pQuat = GetAbsoluteRotation(fBone.ParentName, motion, frame, false);
                        lQuat = pQuat.Inverse * lQuat;
                        // TODO dot product order?
                        //lQuat = lQuat * pQuat.Inverse;
                    }

                    lQuat = ConvertToZUp(lQuat);
                    var lAngles = lQuat.EulerAngles / Math.PI * 180;

                    Console.WriteLine("{0} [FINAL] q: ({1}), angles: ({2})", fBone.Name, lQuat, lAngles);
                }
            }
        }

        private static void HalfSpineTest()
        {
            var motion = MotionSet.HalfSpine;

            foreach (var frame in motion.Frames)
            {
                Console.WriteLine("==================================");
                Console.WriteLine("Frame {0}", frame.Time);

                foreach (var fBone in frame.Bones)
                {
                    var length = (fBone.HeadPos - fBone.TailPos).Length;
                    Console.WriteLine("{0} len: {1}", fBone.Name, length);

                    if (frame.Time == 4 && fBone.Name == "Arm")
                    {
                        if (fBone.ParentName != null)
                        {
                            var dir = GetDirection(fBone.Name, frame);
                            var dDir = GetDirection(fBone.Name, motion);
                            var pDir = GetDirection(fBone.ParentName, frame);
                            var pdDir = GetDirection(fBone.ParentName, motion);

                            var pQuat = FromToRotation(pDir, pdDir);
                            var rot = FromToRotation(pDir, pdDir) * FromToRotation(dDir, dir);
                            var angles = rot.EulerAngles / Math.PI * 180;

                            Console.WriteLine("{0} [ROT] q: ({1}), angles: ({2})", fBone.Name, rot, angles);
                        }
                    }

                    var oQuat = GetOriginalRotation(fBone.Name, motion);

                    var lQuat = GetAbsoluteRotation(fBone.Name, motion, frame, false);
                    if (fBone.ParentName != null)
                    {
                        var pQuat = GetAbsoluteRotation(fBone.ParentName, motion, frame, false);
                        lQuat = pQuat.Inverse * lQuat;
                    }

                    lQuat = ConvertToZUp(lQuat);
                    var lAngles = lQuat.EulerAngles / Math.PI * 180;

                    Console.WriteLine("{0} [FINAL] q: ({1}), angles: ({2})", fBone.Name, lQuat, lAngles);
                }
            }
        }

        private static void HalfSpine5Test()
        {
            var motion = MotionSet.HalfSpine;
            var frame = motion.Frames.Find(f => f.Time == 5);

            var mChest = FindBone(motion, "Chest");
            var fChest = FindBone(frame, "Chest");

            var glChestRot = FromToRotation(mChest.TailPos - mChest.HeadPos, fChest.TailPos - fChest.HeadPos);
            Console.WriteLine("Chest: global rotation: ( {0} ) / ( {1} )", glChestRot, EulerAngles(glChestRot));

            var mShoulder = FindBone(motion, "Shoulder");
            var fShoulder = FindBone(frame, "Shoulder");

            var fShoulderDir = glChestRot.Inverse * (fShoulder.TailPos - fShoulder.HeadPos);
            var gShoulderRot = FromToRotation(mShoulder.TailPos - mShoulder.HeadPos, fShoulderDir);
            Console.WriteLine("Shoulder: global rotation: ( {0} ) / ( {1} )", gShoulderRot, EulerAngles(gShoulderRot));

            var mArm = FindBone(motion, "Arm");
            var fArm = FindBone(frame, "Arm");

            var fArmDir = gShoulderRot.Inverse * (fArm.TailPos - fArm.HeadPos);
            var gArmRot = FromToRotation(mArm.TailPos - mArm.HeadPos, fArmDir);
            Console.WriteLine("Arm: global rotation: ( {0} ) / ( {1} )", gArmRot, EulerAngles(gArmRot));
        }

        private static dvec3 EulerAngles(dquat quat)
        {
            return quat.EulerAngles / Math.PI * 180;
        }

        private static Bone FindBone(Motion motion, string name)
        {
            return motion.Bones.Find(b => b.Name == name);
        }

        private static Bone FindBone(Frame frame, string name)
        {
            return frame.Bones.Find(b => b.Name == name);
        }

        private static dquat GetOriginalRotation(string boneName, Motion motion)
        {
            var bone = motion.Bones.Find(b => b.Name == boneName);
            var dir = (bone.TailPos - bone.HeadPos);

            dquat dquat;
            if (bone.ParentName == null)
            {
                dquat = FromToRotation(new dvec3(0, 0, 1), dir);
            }
            else
            {
                var parentBone = motion.Bones.Find(b => b.Name == bone.ParentName);
                var parentDir = parentBone.TailPos - parentBone.HeadPos;

                dquat = FromToRotation(parentDir, dir);
            }

            Console.WriteLine("{0} [INIT] ({1}), angles ({2})", boneName, dquat, dquat.EulerAngles / Math.PI * 180);

            return dquat;
        }

        private static dvec3 GetDirection(string boneName, Frame frame)
        {
            var bone = frame.Bones.Find(b => b.Name == boneName);

            return (bone.TailPos - bone.HeadPos);
        }

        private static dvec3 GetDirection(string boneName, Motion motion)
        {
            var bone = motion.Bones.Find(b => b.Name == boneName);

            return (bone.TailPos - bone.HeadPos);
        }

        private static dquat GetAbsoluteRotation(string boneName, Motion motion, Frame frame, bool printLog)
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

        private static dvec3 ConvertToZUp(dvec3 vec)
        {
            return vec;
            //return new dvec3(vec.x, -vec.z, vec.y);
        }

        private static dquat ConvertToZUp(dquat dquat)
        {
            return dquat;
            //return new dquat(dquat.x, dquat.z, dquat.y, dquat.w);
        }

        private static dquat FromToRotation(dvec3 from, dvec3 to)
        {
            from = from.Normalized;
            to = to.Normalized;

            var d = dvec3.Dot(from, to);

            if (d >= 1.0)
            {
                // In the case where the two vectors are pointing in the same
                // direction, we simply return the identity rotation.
                return dquat.Identity;
            }
            else if (d <= -1.0)
            {
                // If the two vectors are pointing in opposite directions then we
                // need to supply a quaternion corresponding to a rotation of
                // PI-radians about an axis orthogonal to the fromDirection.
                var axis = dvec3.Cross(from, new dvec3(1, 0, 0));
                if (axis.LengthSqr < 1e-6)
                {
                    // Bad luck. The x-axis and fromDirection are linearly
                    // dependent (colinear). We'll take the axis as the vector
                    // orthogonal to both the y-axis and fromDirection instead.
                    // The y-axis and fromDirection will clearly not be linearly
                    // dependent.
                    axis = dvec3.Cross(from, new dvec3(0, 1, 0));
                }

                // Note that we need to normalize the axis as the cross product of
                // two unit vectors is not nececessarily a unit vector.
                return dquat.FromAxisAngle(Math.PI, axis.Normalized);
            }
            else
            {
                // Scalar component.
                var s = Math.Sqrt(from.LengthSqr * to.LengthSqr) + dvec3.Dot(from, to);

                // Vector component.
                var v = dvec3.Cross(from, to);

                // Return the normalized quaternion rotation.
                var rotation = new dquat(v.x, v.y, v.z, s);

                return rotation.Normalized;
            }
        }

        static void PrintMat4(dmat4 dmat4)
        {
            var builder = new StringBuilder();
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    builder.AppendFormat("{0:f4} ", dmat4[col, row]);
                }

                builder.AppendLine();
            }

            Console.WriteLine(builder.ToString());
        }
    }
}

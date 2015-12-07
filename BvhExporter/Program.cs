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
            var motion = MotionSet.Simple1;

            foreach (var frame in motion.Frames)
            {
                foreach (var boneDef in motion.Bones)
                {
                    Console.WriteLine("Frame {0} bone {1}", frame.Time, boneDef.Name);

                    var trans = mat4.Translate(boneDef.HeadPos);
                    var itrans = trans.Inverse;

                    Console.WriteLine("trans");
                    PrintMat4(trans);

                    Console.WriteLine("itrans");
                    PrintMat4(itrans);

                    var boneFra = frame.Bones.Find(b => b.Name == boneDef.Name);

                    var matDef = boneDef.Rotation.ToMat4;
                    var matFra = boneFra.Rotation.ToMat4;

                    Console.WriteLine("matDef");
                    PrintMat4(matDef);

                    Console.WriteLine("matFra");
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
                    Console.WriteLine("angles {0} vs {1}", angles * Math.PI, boneFra.RotationAngles);
                }
            }

            Console.ReadKey();
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

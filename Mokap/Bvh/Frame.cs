using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Mokap.Bvh
{
    class Frame
    {
        private List<Vector3D> values = new List<Vector3D>();

        public Frame(Vector3D translation, IEnumerable<Vector3D> rotations)
        {
            this.values.Add(translation);
            this.values.AddRange(rotations);
        }

        IEnumerable<double> FlattenVector3D(Vector3D vector)
        {
            yield return vector.X;
            yield return vector.Y;
            yield return vector.Z;
        }

        public double[] Values
        {
            get { return values.SelectMany(FlattenVector3D).ToArray(); }
        }
    }
}

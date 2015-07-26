using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;

namespace Mokap
{
    class CsvBodyAdapter : IBodyAdapter
    {
        private int frameId;
        private Joint[] joints = new Joint[Enum.GetValues(typeof(JointType)).Length];

        CsvBodyAdapter(int frameId)
        {
            this.frameId = frameId;
        }

        public static IEnumerable<CsvBodyAdapter> ParseFromCsvFile(string filename)
        {
            CsvBodyAdapter body = null;
            foreach (var line in File.ReadAllLines(filename))
            {
                var columns = line.Split(',');
                var frameId = int.Parse(columns[0]);
                var jointType = (JointType)Enum.Parse(typeof(JointType), columns[1]);
                var position = new Vector3D(
                    double.Parse(columns[2]),
                    double.Parse(columns[3]),
                    double.Parse(columns[4])
                );
                var rotation = new Quaternion(
                    double.Parse(columns[5]),
                    double.Parse(columns[6]),
                    double.Parse(columns[7]),
                    double.Parse(columns[8])
                );

                if (body == null )
                {
                    body = new CsvBodyAdapter(frameId);
                }
                else if (body.frameId != frameId)
                {
                    yield return body;
                    body = new CsvBodyAdapter(frameId);
                }

                body.joints[(int)jointType] = new Joint(jointType, position, rotation);
            }

            if (body != null)
                yield return body;
        }

        #region IBodyAdapter

        Vector3D IBodyAdapter.GetJointPosition(JointType type)
        {
            return joints[(int)type].Position;
        }

        Quaternion IBodyAdapter.GetJointRotation(JointType type)
        {
            return joints[(int)type].Rotation;
        }

        TrackingState IBodyAdapter.GetJointState(JointType type)
        {
            return TrackingState.Tracked;
        }

        #endregion

        #region Joint Data

        class Joint
        {
            public Joint(JointType jointType, Vector3D position, Quaternion rotation)
            {
                this.JointType = jointType;
                this.Position = position;
                this.Rotation = rotation;
            }

            public JointType JointType { get; private set; }
            public Vector3D Position { get; private set; }
            public Quaternion Rotation { get; private set; }
        }

        #endregion
    }
}

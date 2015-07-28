using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mokap.Demo
{
    public class Frame
    {
        const float ScaleFactor = 10;

        private int frameId;
        private List<Joint> joints = new List<Joint>();

        Frame(int frameId)
        {
            this.frameId = frameId;
        }

        public static IEnumerable<Frame> ParseFromCsvFile(string filename)
        {
            var asset = Resources.Load(filename) as TextAsset;
            if (asset == null)
            {
                Debug.LogError(string.Format("Missing csv file: {0}", filename));
                yield break;
            }

            Frame body = null;
            foreach (var line in asset.text.Split(null))
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var columns = line.Split(',');
                var frameId = int.Parse(columns[0]);
                var jointType = columns[1];
                var position = new Vector3(
                    float.Parse(columns[2]) * ScaleFactor,
                    float.Parse(columns[3]) * ScaleFactor,
                    float.Parse(columns[4]) * ScaleFactor
                );
                var rotation = new Quaternion(
                    float.Parse(columns[5]),
                    float.Parse(columns[6]),
                    float.Parse(columns[7]),
                    float.Parse(columns[8])
                );

                if (body == null)
                {
                    body = new Frame(frameId);
                }
                else if (body.frameId != frameId)
                {
                    yield return body;
                    body = new Frame(frameId);
                }

                body.joints.Add(new Joint(jointType, position, rotation));
            }

            if (body != null)
                yield return body;
        }

        public Joint FindJoint(string type)
        {
            return this.joints.Find(j => j.Type == type);
        }

        #region Joint Data

        public class Joint
        {
            public Joint(string jointType, Vector3 position, Quaternion rotation)
            {
                this.Type = jointType;
                this.Position = position;
                this.Rotation = rotation;
            }

            public string Type { get; private set; }
            public Vector3 Position { get; private set; }
            public Quaternion Rotation { get; private set; }
        }

        #endregion

        public IEnumerable<Joint> Joints
        {
            get { return this.joints; }
        }
    }
}

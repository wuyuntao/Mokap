using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Mokap.Data
{

    [Serializable]
    sealed class BodyFrameData
    {
        public TimeSpan RelativeTime;

        public Body[] Bodies;

        public static BodyFrameData CreateFromKinectSensor(BodyFrameReference frameRef)
        {
            using (var frame = frameRef.AcquireFrame())
            {
                if (frame == null)
                {
                    return null;
                }

                // TODO: Avoid allocate body array every time
                var coordinateMapper = frame.BodyFrameSource.KinectSensor.CoordinateMapper;
                var bodies = new Microsoft.Kinect.Body[frame.BodyCount];
                frame.GetAndRefreshBodyData(bodies);

                return new BodyFrameData()
                {
                    RelativeTime = frame.RelativeTime,
                    Bodies = Array.ConvertAll(bodies, b => CreateBody(b, coordinateMapper)),
                };
            }
        }

        private static Body CreateBody(Microsoft.Kinect.Body input, CoordinateMapper coordinateMapper)
        {
            var output = new Body()
            {
                TrackingId = input.TrackingId,
                IsTracked = input.IsTracked,
                IsRestricted = input.IsRestricted,
                ClippedEdges = input.ClippedEdges,

                HandLeft = new Hand()
                {
                    Confidence = input.HandLeftConfidence,
                    State = input.HandLeftState,
                },

                HandRight = new Hand()
                {
                    Confidence = input.HandRightConfidence,
                    State = input.HandRightState,
                },

                Joints = new Dictionary<JointType, Joint>(),
            };

            var jointTypes = Enum.GetValues(typeof(JointType));

            foreach (JointType type in jointTypes)
            {
                var joint = input.Joints[type];
                var position2d = coordinateMapper.MapCameraPointToDepthSpace(joint.Position);
                var position3d = joint.Position;
                var orientation = input.JointOrientations[type].Orientation;

                output.Joints.Add(type, new Joint()
                {
                    Type = type,
                    State = joint.TrackingState,
                    Position2D = new Point(position2d.X, position2d.Y),
                    Position3D = new Vector3D(position3d.X, position3d.Y, position3d.Z),
                    Rotation = new Quaternion(orientation.X, orientation.Y, orientation.Z, orientation.W),
                });
            }

            return output;
        }

        [Serializable]
        public sealed class Body
        {
            public ulong TrackingId;

            public bool IsTracked;

            public bool IsRestricted;

            public FrameEdges ClippedEdges;

            public Hand HandLeft;

            public Hand HandRight;

            public Dictionary<JointType, Joint> Joints;
        }

        [Serializable]
        public sealed class Hand
        {
            public TrackingConfidence Confidence;

            public HandState State;
        }

        [Serializable]
        public sealed class Joint
        {
            public JointType Type;

            public TrackingState State;

            public Point Position2D;

            public Vector3D Position3D;

            public Quaternion Rotation;
        }
    }
}

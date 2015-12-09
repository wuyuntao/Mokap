using Microsoft.Kinect;
using Mokap.Data;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Mokap.Kinect
{
    static class BodyFrameDataConverter
    {
        public static BodyFrameData CreateData(this BodyFrameReference frameRef, TimeSpan relativeTime)
        {
            using (var frame = frameRef.AcquireFrame())
            {
                if (frame == null)
                {
                    return null;
                }

                // TODO: Avoid allocate body array every time
                var coordinateMapper = frame.BodyFrameSource.KinectSensor.CoordinateMapper;
                var bodies = new Body[frame.BodyCount];
                frame.GetAndRefreshBodyData(bodies);

                return new BodyFrameData()
                {
                    RelativeTime = relativeTime,
                    Bodies = Array.ConvertAll(bodies, b => CreateBody(b, coordinateMapper)),
                };
            }
        }

        private static BodyFrameData.Body CreateBody(Body input, CoordinateMapper coordinateMapper)
        {
            var output = new BodyFrameData.Body()
            {
                TrackingId = input.TrackingId,
                IsTracked = input.IsTracked,
                IsRestricted = input.IsRestricted,
                ClippedEdges = (Schemas.RecorderMessages.FrameEdges)(int)input.ClippedEdges,

                HandLeft = new BodyFrameData.Hand()
                {
                    Confidence = (Schemas.RecorderMessages.TrackingConfidence)(int)input.HandLeftConfidence,
                    State = (Schemas.RecorderMessages.HandState)(int)input.HandLeftState,
                },

                HandRight = new BodyFrameData.Hand()
                {
                    Confidence = (Schemas.RecorderMessages.TrackingConfidence)(int)input.HandRightConfidence,
                    State = (Schemas.RecorderMessages.HandState)(int)input.HandRightState,
                },

                Joints = new Dictionary<Schemas.RecorderMessages.JointType, BodyFrameData.Joint>(),
            };

            var jointTypes = Enum.GetValues(typeof(JointType));

            foreach (JointType type in jointTypes)
            {
                var joint = input.Joints[type];
                var position2d = coordinateMapper.MapCameraPointToDepthSpace(joint.Position);
                var position3d = joint.Position;
                var orientation = input.JointOrientations[type].Orientation;

                output.Joints.Add((Schemas.RecorderMessages.JointType)(int)type, new BodyFrameData.Joint()
                {
                    Type = (Schemas.RecorderMessages.JointType)(int)type,
                    State = (Schemas.RecorderMessages.TrackingState)(int)joint.TrackingState,
                    Position2D = new Point(position2d.X, position2d.Y),
                    Position3D = new Vector3D(position3d.X, position3d.Y, position3d.Z),
                    Rotation = new Quaternion(orientation.X, orientation.Y, orientation.Z, orientation.W),
                });
            }

            return output;
        }
    }
}

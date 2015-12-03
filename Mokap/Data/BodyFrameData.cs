using FlatBuffers;
using FlatBuffers.Schema;
using Microsoft.Kinect;
using Mokap.Schemas.RecorderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using BodyFrameDataMsg = Mokap.Schemas.RecorderMessages.BodyFrameData;
using BodyMsg = Mokap.Schemas.RecorderMessages.Body;
using HandMsg = Mokap.Schemas.RecorderMessages.Hand;
using JointMsg = Mokap.Schemas.RecorderMessages.Joint;

namespace Mokap.Data
{

    [Serializable]
    sealed class BodyFrameData
    {
        public TimeSpan RelativeTime;

        public Body[] Bodies;

        public static BodyFrameData CreateFromKinectSensor(BodyFrameReference frameRef, TimeSpan relativeTime)
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
                    RelativeTime = relativeTime,
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

        public static BodyFrameData Deserialize(BodyFrameDataMsg message)
        {
            var frame = new BodyFrameData()
            {
                RelativeTime = new TimeSpan(message.RelativeTime),
                Bodies = new Body[message.BodiesLength],
            };

            for (int i = 0; i < message.BodiesLength; i++)
            {
                frame.Bodies[i] = Body.Deserialize(message.GetBodies(i));
            }

            return frame;
        }

        public byte[] Serialize()
        {
            var fbb = new FlatBufferBuilder(1024);

            var bodies = Array.ConvertAll(Bodies, body => body.Serialize(fbb));
            var msg = BodyFrameDataMsg.CreateBodyFrameData(fbb,
                    RelativeTime.Ticks,
                    BodyFrameDataMsg.CreateBodiesVector(fbb, bodies));
            fbb.Finish(msg.Value);

            return fbb.ToProtocolMessage(MessageIds.BodyFrameData);
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

            public static Body Deserialize(BodyMsg message)
            {
                var body = new Body()
                {
                    TrackingId = message.TrackingId,
                    IsTracked = message.IsTracked,
                    IsRestricted = message.IsRestricted,
                    ClippedEdges = (FrameEdges)message.ClippedEdges,

                    HandLeft = Hand.Deserialize(message.HandLeft),
                    HandRight = Hand.Deserialize(message.HandRight),

                    Joints = new Dictionary<JointType, Joint>(),
                };

                for (int i = 0; i < message.JointsLength; i++)
                {
                    var joint = Joint.Deserialize(message.GetJoints(i));

                    body.Joints.Add(joint.Type, joint);
                }

                return body;
            }

            public Offset<BodyMsg> Serialize(FlatBufferBuilder fbb)
            {
                var joints = Joints.Values.Select(joint => joint.Serialize(fbb)).ToArray();

                return BodyMsg.CreateBody(fbb,
                        TrackingId, IsTracked, IsTracked, (int)ClippedEdges,
                        HandLeft.Serialize(fbb), HandRight.Serialize(fbb),
                        BodyMsg.CreateJointsVector(fbb, joints));
            }
        }

        [Serializable]
        public sealed class Hand
        {
            public TrackingConfidence Confidence;

            public HandState State;

            public static Hand Deserialize(HandMsg message)
            {
                return new Hand()
                {
                    State = (HandState)message.State,
                    Confidence = (TrackingConfidence)message.Confidence,
                };
            }

            public Offset<HandMsg> Serialize(FlatBufferBuilder fbb)
            {
                return HandMsg.CreateHand(fbb, (int)Confidence, (int)State);
            }
        }

        [Serializable]
        public sealed class Joint
        {
            public JointType Type;

            public TrackingState State;

            public Point Position2D;

            public Vector3D Position3D;

            public Quaternion Rotation;

            public static Joint Deserialize(JointMsg message)
            {
                return new Joint()
                {
                    Type = (JointType)message.Type,
                    State = (TrackingState)message.State,
                    Position2D = new Point(message.Position2D.X, message.Position2D.Y),
                    Position3D = new Vector3D(message.Position3D.X, message.Position3D.Y, message.Position3D.Z),
                    Rotation = new Quaternion(message.Rotation.X, message.Rotation.Y, message.Rotation.Z, message.Rotation.W),
                };
            }


            public Offset<JointMsg> Serialize(FlatBufferBuilder fbb)
            {
                var position2D = Vector2.CreateVector2(fbb, Position2D.X, Position2D.Y);
                var position3D = Vector3.CreateVector3(fbb, Position3D.X, Position3D.Y, Position3D.Z);
                var rotation = Schemas.RecorderMessages.Vector4.CreateVector4(fbb, Rotation.X, Rotation.Y, Rotation.Z, Rotation.W);

                return JointMsg.CreateJoint(fbb, (int)Type, (int)State, position2D, position3D, rotation);
            }
        }
    }
}

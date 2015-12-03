using FlatBuffers;
using FlatBuffers.Schema;
using Microsoft.Kinect;
using Mokap.Schemas.RecorderMessages;
using System;
using DepthFrameDataMsg = Mokap.Schemas.RecorderMessages.DepthFrameData;

namespace Mokap.Data
{
    [Serializable]
    sealed class DepthFrameData
    {
        public TimeSpan RelativeTime;

        public int Width;

        public int Height;

        public ushort[] Data;

        public ushort MinReliableDistance;

        public ushort MaxReliableDistance;

        public static DepthFrameData CreateFromKinectSensor(DepthFrameReference frameRef)
        {
            using (var frame = frameRef.AcquireFrame())
            {
                if (frame == null)
                {
                    return null;
                }

                var frameDesc = frame.FrameDescription;

                // TODO: Avoid allocate ushort array every time
                var data = new ushort[frameDesc.Width * frameDesc.Height];
                frame.CopyFrameDataToArray(data);

                return new DepthFrameData()
                {
                    RelativeTime = frame.RelativeTime,
                    Width = frameDesc.Width,
                    Height = frameDesc.Height,
                    Data = data,
                    MinReliableDistance = frame.DepthMinReliableDistance,
                    MaxReliableDistance = frame.DepthMaxReliableDistance,
                };
            }
        }

        public static DepthFrameData Deserialize(DepthFrameDataMsg message)
        {
            var frame = new DepthFrameData()
            {
                RelativeTime = new TimeSpan(message.RelativeTime),
                Width = message.Width,
                Height = message.Height,
                MinReliableDistance = message.MinReliableDistance,
                MaxReliableDistance = message.MaxReliableDistance,
                Data = new ushort[message.DataLength],
            };

            for (int i = 0; i < message.DataLength; i++)
            {
                frame.Data[i] = message.GetData(i);
            }

            return frame;
        }

        public byte[] Serialize()
        {
            var fbb = new FlatBufferBuilder(0);

            var data = DepthFrameDataMsg.CreateDataVector(fbb, Data);
            var msg = DepthFrameDataMsg.CreateDepthFrameData(fbb,
                    RelativeTime.Ticks,
                    Width, Height,
                    data,
                    MinReliableDistance, MaxReliableDistance);
            fbb.Finish(msg.Value);

            return fbb.ToProtocolMessage(MessageIds.DepthFrameData);
        }
    }
}

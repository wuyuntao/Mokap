using FlatBuffers;
using FlatBuffers.Schema;
using Microsoft.Kinect;
using Mokap.Schemas.RecorderMessages;
using System;
using ColorFrameDataMsg = Mokap.Schemas.RecorderMessages.ColorFrameData;

namespace Mokap.Data
{
    [Serializable]
    sealed class ColorFrameData
    {
        public TimeSpan RelativeTime;

        public int Width;

        public int Height;

        public byte[] Data;

        public static ColorFrameData CreateFromKinectSensor(ColorFrameReference frameRef)
        {
            using (var frame = frameRef.AcquireFrame())
            {
                if (frame == null)
                {
                    return null;
                }

                var frameDesc = frame.FrameDescription;

                // TODO: Avoid allocate byte array every time
                var data = new byte[frameDesc.Width * frameDesc.Height * sizeof(int)];
                // TODO: Check if Bgra can be written to bitmap directly
                frame.CopyConvertedFrameDataToArray(data, ColorImageFormat.Bgra);

                return new ColorFrameData()
                {
                    RelativeTime = frame.RelativeTime,
                    Width = frameDesc.Width,
                    Height = frameDesc.Height,
                    Data = data,
                };
            }
        }

        public static ColorFrameData Deserialize(ColorFrameDataMsg message)
        {
            var frame = new ColorFrameData()
            {
                RelativeTime = new TimeSpan(message.RelativeTime),
                Width = message.Width,
                Height = message.Height,
                Data = new byte[message.DataLength],
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

            var data = ColorFrameDataMsg.CreateDataVector(fbb, Data);
            var msg = ColorFrameDataMsg.CreateColorFrameData(fbb,
                    RelativeTime.Ticks,
                    Width, Height,
                    data);
            fbb.Finish(msg.Value);

            return fbb.ToProtocolMessage(MessageIds.ColorFrameData);
        }
    }
}
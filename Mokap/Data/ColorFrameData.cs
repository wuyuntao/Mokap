using Microsoft.Kinect;
using System;

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
    }
}
using Microsoft.Kinect;
using System;

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
    }
}

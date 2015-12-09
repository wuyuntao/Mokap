using Microsoft.Kinect;
using Mokap.Data;

namespace Mokap.Kinect
{
    static class DepthFrameDataConverter
    {
        public static DepthFrameData CreateData(this DepthFrameReference frameRef)
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

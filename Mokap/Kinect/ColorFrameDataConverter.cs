using Microsoft.Kinect;
using Mokap.Data;

namespace Mokap.Kinect
{
    static class ColorFrameDataConverter
    {
        public static ColorFrameData CreateData(this ColorFrameReference frameRef)
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

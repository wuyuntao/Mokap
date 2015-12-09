using Microsoft.Kinect;
using Mokap.Data;

namespace Mokap.Kinect
{
    static class MetadataFactory
    {
        public static Metadata Create()
        {
            var sensor = KinectSensor.GetDefault();
            if (!sensor.IsOpen)
            {
                sensor.Open();
            }

            var colorFrameDescription = sensor.ColorFrameSource.FrameDescription;
            var depthFrameDescription = sensor.DepthFrameSource.FrameDescription;

            return new Metadata()
            {
                ColorFrameWidth = colorFrameDescription.Width,
                ColorFrameHeight = colorFrameDescription.Height,
                DepthFrameWidth = depthFrameDescription.Width,
                DepthFrameHeight = depthFrameDescription.Height,
            };
        }

    }
}

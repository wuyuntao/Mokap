using Microsoft.Kinect;
using System;

namespace Mokap.Data
{
    [Serializable]
    sealed class Metadata
    {
        public int ColorFrameWidth;

        public int ColorFrameHeight;

        public int DepthFrameWidth;

        public int DepthFrameHeight;

        public static Metadata GetFromKinectSensor()
        {
            var sensor = KinectSensor.GetDefault();
            if (!sensor.IsAvailable)
            {
                return null;
            }

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
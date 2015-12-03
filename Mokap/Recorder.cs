using Microsoft.Kinect;
using Mokap.Data;
using NLog;

namespace Mokap
{
    sealed class Recorder : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private KinectSensor sensor = KinectSensor.GetDefault();

        private BodyFrameReader bodyReader;

        private MultiSourceFrameReader colorReader;

        public Recorder()
        {
            if (!sensor.IsOpen)
            {
                sensor.Open();
            }

            if (sensor.IsOpen)
            {
                bodyReader = sensor.BodyFrameSource.OpenReader();
                colorReader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth | FrameSourceTypes.Color);

                logger.Trace("Kinect sensor is open");
            }
            else
            {
                logger.Error("Kinect sensor is not open");
            }
        }

        public void Start()
        {
            bodyReader.FrameArrived += BodyReader_FrameArrived;
            colorReader.MultiSourceFrameArrived += ColorReader_MultiSourceFrameArrived;
        }

        public void Stop()
        {
            bodyReader.FrameArrived -= BodyReader_FrameArrived;
            colorReader.MultiSourceFrameArrived -= ColorReader_MultiSourceFrameArrived;
        }

        protected override void DisposeManaged()
        {
            if (sensor != null)
            {
                sensor.Close();
                sensor = null;
            }

            base.DisposeManaged();
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var bodyFrame = BodyFrameData.CreateFromKinectSensor(e.FrameReference);
            if (bodyFrame != null)
            {
                // TODO raise as event

                logger.Trace("Update body frame: {0}", bodyFrame);
            }
        }

        private void ColorReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var multiSourceFrame = e.FrameReference.AcquireFrame();
            if (multiSourceFrame == null)
            {
                logger.Trace("Abort update since MultiSourceFrame is null");
                return;
            }

            var colorFrame = ColorFrameData.CreateFromKinectSensor(multiSourceFrame.ColorFrameReference);
            if (colorFrame != null)
            {
                // TODO raise as event

                logger.Trace("Update color frame: {0}", colorFrame);
            }

            var depthFrame = DepthFrameData.CreateFromKinectSensor(multiSourceFrame.DepthFrameReference);
            if (depthFrame != null)
            {
                // TODO raise as event

                logger.Trace("Update depth frame: {0}", depthFrame);
            }
        }
    }
}
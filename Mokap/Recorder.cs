using Microsoft.Kinect;
using Mokap.States;
using NLog;

namespace Mokap
{
    sealed class Recorder : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private KinectSensor sensor = KinectSensor.GetDefault();

        private BodyFrameReader bodyReader;

        private MultiSourceFrameReader colorReader;

        private ColorCamera colorFrame;

        private DepthCamera depthFrame;

        private BodyCamera bodyFrame;

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

                //var colorFrameDescription = sensor.ColorFrameSource.FrameDescription;
                //colorFrame = new ColorCamera(colorFrameDescription.Width, colorFrameDescription.Height);

                //var depthFrameDescription = sensor.DepthFrameSource.FrameDescription;
                //depthFrame = new DepthCamera(depthFrameDescription.Width, depthFrameDescription.Height);

                //bodyFrame = new BodyCamera(sensor.CoordinateMapper, depthFrameDescription.Width, depthFrameDescription.Height);

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
            var bodyFrameChanged = bodyFrame.Update(e.FrameReference);

            logger.Trace("Update frames. Body: {0}", bodyFrameChanged);
        }

        private void ColorReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var multiSourceFrame = e.FrameReference.AcquireFrame();
            if (multiSourceFrame == null)
            {
                logger.Trace("Abort update since MultiSourceFrame is null");
                return;
            }

            //var colorFrameChanged = colorFrame.Update(multiSourceFrame.ColorFrameReference);
            //var depthFrameChanged = depthFrame.Update(multiSourceFrame.DepthFrameReference);

            //logger.Trace("Update frames. Color: {0}, Depth: {1}", colorFrameChanged, depthFrameChanged);
        }

        #region Properties

        public ColorCamera ColorFrame
        {
            get { return colorFrame; }
        }

        public DepthCamera DepthFrame
        {
            get { return depthFrame; }
        }

        public BodyCamera BodyFrame
        {
            get { return bodyFrame; }
        }

        #endregion
    }
}
using Microsoft.Kinect;
using NLog;

namespace Mokap
{
    sealed class Recorder : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private KinectSensor sensor = KinectSensor.GetDefault();

        private BodyFrameReader bodyReader;

        private MultiSourceFrameReader colorReader;

        private ColorFrameData colorFrame;

        private DepthFrameData depthFrame;

        private BodyFrameData bodyFrame;

        public Recorder()
        {
            if (!this.sensor.IsOpen)
            {
                this.sensor.Open();
            }

            if (this.sensor.IsOpen)
            {
                this.bodyReader = this.sensor.BodyFrameSource.OpenReader();
                this.colorReader = this.sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth | FrameSourceTypes.Color);

                var colorFrameDescription = this.sensor.ColorFrameSource.FrameDescription;
                this.colorFrame = new ColorFrameData(colorFrameDescription.Width, colorFrameDescription.Height);

                var depthFrameDescription = this.sensor.DepthFrameSource.FrameDescription;
                this.depthFrame = new DepthFrameData(depthFrameDescription.Width, depthFrameDescription.Height);

                this.bodyFrame = new BodyFrameData(this.sensor.CoordinateMapper, depthFrameDescription.Width, depthFrameDescription.Height);

                logger.Trace("Kinect sensor is open");
            }
            else
            {
                logger.Error("Kinect sensor is not open");
            }
        }

        public void Start()
        {
            this.bodyReader.FrameArrived += BodyReader_FrameArrived;
            this.colorReader.MultiSourceFrameArrived += ColorReader_MultiSourceFrameArrived;
        }

        public void Stop()
        {
            this.bodyReader.FrameArrived -= BodyReader_FrameArrived;
            this.colorReader.MultiSourceFrameArrived -= ColorReader_MultiSourceFrameArrived;
        }

        protected override void DisposeManaged()
        {
            if (this.sensor != null)
            {
                this.sensor.Close();
                this.sensor = null;
            }

            base.DisposeManaged();
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var bodyFrameChanged = this.bodyFrame.Update(e.FrameReference);

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

            var colorFrameChanged = this.colorFrame.Update(multiSourceFrame.ColorFrameReference);
            var depthFrameChanged = this.depthFrame.Update(multiSourceFrame.DepthFrameReference);

            logger.Trace("Update frames. Color: {0}, Depth: {1}", colorFrameChanged, depthFrameChanged);
        }

        #region Properties

        public ColorFrameData ColorFrame
        {
            get { return this.colorFrame; }
        }

        public DepthFrameData DepthFrame
        {
            get { return this.depthFrame; }
        }

        public BodyFrameData BodyFrame
        {
            get { return this.bodyFrame; }
        }

        #endregion
    }
}
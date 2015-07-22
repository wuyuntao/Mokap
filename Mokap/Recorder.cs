using Microsoft.Kinect;
using Mokap.KinectUtils;
using NLog;

namespace Mokap
{
    sealed class Recorder : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private KinectSensor sensor = KinectSensor.GetDefault();

        private MultiSourceFrameReader reader;

        private ColorFrameData colorFrame;

        private DepthFrameData depthFrame;

        public Recorder()
        {
            if (!this.sensor.IsOpen)
            {
                this.sensor.Open();
            }

            if (this.sensor.IsOpen)
            {
                this.reader = this.sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth | FrameSourceTypes.Color);

                var colorFrameDescription = this.sensor.ColorFrameSource.FrameDescription;
                this.colorFrame = new ColorFrameData(colorFrameDescription.Width, colorFrameDescription.Height);

                var depthFrameDescription = this.sensor.DepthFrameSource.FrameDescription;
                this.depthFrame = new DepthFrameData(depthFrameDescription.Width, depthFrameDescription.Height);

                this.reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                logger.Trace("Kinect sensor is open");
            }
            else
            {
                logger.Error("Kinect sensor is not open");
            }
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

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var multiSourceFrame = e.FrameReference.AcquireFrame();
            if (multiSourceFrame == null)
            {
                logger.Trace("Abort update since MultiSourceFrame is null");
                return;
            }

            var colorFrameChanged = this.colorFrame.Update(multiSourceFrame);
            var depthFrameChanged = this.depthFrame.Update(multiSourceFrame);

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

        #endregion
    }
}
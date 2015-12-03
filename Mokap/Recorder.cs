using Microsoft.Kinect;
using Mokap.Data;
using NLog;
using System;
using System.IO;
using System.Windows.Threading;

namespace Mokap
{
    sealed class Recorder : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<BodyFrameUpdatedEventArgs> BodyFrameUpdated;

        public event EventHandler<ColorFrameUpdatedEventArgs> ColorFrameUpdated;

        public event EventHandler<DepthFrameUpdatedEventArgs> DepthFrameUpdated;

        private KinectSensor sensor = KinectSensor.GetDefault();

        private BodyFrameReader bodyReader;

        private MultiSourceFrameReader colorReader;

        private FileStream fileStream;

        private long fileStreamOffset;

        private bool started;

        private Metadata metadata;

        private Dispatcher dispatcher;

        public Recorder(string filename, Dispatcher dispatcher)
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

            metadata = Metadata.CreateFromKinectSensor();

            fileStream = new FileStream(filename, FileMode.Create);
            AppendMessageToFileStream(metadata.Serialize());

            this.dispatcher = dispatcher;
        }

        public void Start()
        {
            if (!started)
            {
                bodyReader.FrameArrived += BodyReader_FrameArrived;
                colorReader.MultiSourceFrameArrived += ColorReader_MultiSourceFrameArrived;

                started = true;
            }
        }

        public void Stop()
        {
            if (started)
            {
                bodyReader.FrameArrived -= BodyReader_FrameArrived;
                colorReader.MultiSourceFrameArrived -= ColorReader_MultiSourceFrameArrived;

                started = false;
            }
        }

        protected override void DisposeManaged()
        {
            Stop();

            if (sensor != null)
            {
                sensor.Close();
                sensor = null;
            }

            // Wait for all data is flushed to file stream
            var task = fileStream.FlushAsync();
            task.Wait();

            fileStream.Close();

            base.DisposeManaged();
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var bodyFrame = BodyFrameData.CreateFromKinectSensor(e.FrameReference);
            if (bodyFrame != null)
            {
                logger.Trace("Update body frame: {0}", bodyFrame);

                AppendMessageToFileStream(bodyFrame.Serialize());

                if (BodyFrameUpdated != null)
                {
                    BodyFrameUpdated(this, new BodyFrameUpdatedEventArgs(bodyFrame));
                }
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
                logger.Trace("Update color frame: {0}", colorFrame);

                AppendMessageToFileStream(colorFrame.Serialize());

                if (ColorFrameUpdated != null)
                {
                    ColorFrameUpdated(this, new ColorFrameUpdatedEventArgs(colorFrame));
                }
            }

            var depthFrame = DepthFrameData.CreateFromKinectSensor(multiSourceFrame.DepthFrameReference);
            if (depthFrame != null)
            {
                logger.Trace("Update depth frame: {0}", depthFrame);

                AppendMessageToFileStream(depthFrame.Serialize());

                if (DepthFrameUpdated != null)
                {
                    DepthFrameUpdated(this, new DepthFrameUpdatedEventArgs(depthFrame));
                }
            }
        }

        private void AppendMessageToFileStream(byte[] bytes)
        {
            fileStream.Seek(fileStreamOffset, SeekOrigin.Begin);
            fileStreamOffset += bytes.Length;

            fileStream.WriteAsync(bytes, 0, bytes.Length);
        }

        public Metadata Metadata
        {
            get { return metadata; }
        }
    }
}
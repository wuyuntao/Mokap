﻿using Microsoft.Kinect;
using Mokap.Data;
using NLog;
using System;
using System.IO;

namespace Mokap
{
    sealed class Recorder : Disposable
    {
        public event EventHandler<BodyFrameUpdatedEventArgs> BodyFrameUpdated;

        public event EventHandler<ColorFrameUpdatedEventArgs> ColorFrameUpdated;

        public event EventHandler<DepthFrameUpdatedEventArgs> DepthFrameUpdated;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private KinectSensor sensor = KinectSensor.GetDefault();

        private BodyFrameReader bodyReader;

        private MultiSourceFrameReader colorReader;

        private FileStream fileStream;

        private long fileStreamOffset;

        private bool started;

        public Recorder(string filename)
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

            fileStream = new FileStream(filename, FileMode.Create);
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

                // TODO Write serialized frame data

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

                // TODO Write serialized frame data

                if (ColorFrameUpdated != null)
                {
                    ColorFrameUpdated(this, new ColorFrameUpdatedEventArgs(colorFrame));
                }
            }

            var depthFrame = DepthFrameData.CreateFromKinectSensor(multiSourceFrame.DepthFrameReference);
            if (depthFrame != null)
            {
                logger.Trace("Update depth frame: {0}", depthFrame);

                // TODO Write serialized frame data

                if (DepthFrameUpdated != null)
                {
                    DepthFrameUpdated(this, new DepthFrameUpdatedEventArgs(depthFrame));
                }
            }
        }

        private void AppendBytesToFileStream(byte[] bytes)
        {
            fileStream.Seek(fileStreamOffset, SeekOrigin.Begin);
            fileStreamOffset += bytes.Length;

            fileStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
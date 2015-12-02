using Microsoft.Kinect;
using Microsoft.Win32;
using Mokap.Bvh;
using Mokap.Data;
using System;
using System.Windows;

namespace Mokap.States
{
    class Recording : Capturing
    {
        private Recorder recorder;

        public Recording(MainWindow mainWindow)
            : base(mainWindow, GetMetadataFromKinectSensor())
        {
            mainWindow.RecordButton.Content = "Stop Record";
            mainWindow.RecordButton.IsEnabled = true;
            mainWindow.RecordButton.Click += RecordButton_Click;

            recorder = new Recorder();
            recorder.Start();
        }

        protected override void DisposeManaged()
        {
            MainWindow.RecordButton.Click -= RecordButton_Click;

            SafeDispose(ref recorder);

            base.DisposeManaged();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            recorder.Stop();

            if (recorder.BodyFrame.Motion.FrameCount > 0)
            {
                var dialog = new SaveFileDialog()
                {
                    FileName = string.Format("Mokap_{0}.bvh", DateTime.Now.ToString("yyyyMMdd_HHmmss")),
                    Filter = "Biovision Hierarchy Files|*.bvh",
                };

                if (dialog.ShowDialog() == true)
                {
                    BvhWriter.Write(dialog.FileName, recorder.BodyFrame.Motion);

                    logger.Trace("Record to bvh file {0}", dialog.FileName);
                }
                else
                {
                    logger.Trace("User cancelled saving bvh file");
                }
            }
            else
            {
                logger.Trace("Zero frame to record");
            }

            Become(new Idle(MainWindow));
        }


        private static MetaData GetMetadataFromKinectSensor()
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

            return new MetaData()
            {
                ColorFrameWidth = colorFrameDescription.Width,
                ColorFrameHeight = colorFrameDescription.Height,
                DepthFrameWidth = depthFrameDescription.Width,
                DepthFrameHeight = depthFrameDescription.Height,
            };
        }
    }
}
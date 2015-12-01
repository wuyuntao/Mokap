using Microsoft.Win32;
using Mokap.Bvh;
using System;
using System.Windows;

namespace Mokap.States
{
    class Record : MainWindow.State
    {
        private Recorder recorder;

        public Record(MainWindow mainWindow)
            : base(mainWindow)
        {
            mainWindow.RecordButton.Content = "Stop Record";
            mainWindow.RecordButton.IsEnabled = true;
            mainWindow.RecordButton.Click += RecordButton_Click;

            recorder = new Recorder();
            recorder.Start();

            mainWindow.ColorCamera.Source = recorder.ColorFrame.Bitmap;
            mainWindow.DepthCamera.Source = recorder.DepthFrame.Bitmap;
            mainWindow.BodyCamera.Source = recorder.BodyFrame.Bitmap;
        }

        protected override void DisposeManaged()
        {
            MainWindow.RecordButton.Click -= RecordButton_Click;

            MainWindow.ColorCamera.Source = null;
            MainWindow.DepthCamera.Source = null;
            MainWindow.BodyCamera.Source = null;

            SafeDispose(ref recorder);

            base.DisposeManaged();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            this.recorder.Stop();

            if (this.recorder.BodyFrame.Motion.FrameCount > 0)
            {
                var dialog = new SaveFileDialog()
                {
                    FileName = string.Format("Mokap_{0}.bvh", DateTime.Now.ToString("yyyyMMdd_HHmmss")),
                    Filter = "Biovision Hierarchy Files|*.bvh",
                };

                if (dialog.ShowDialog() == true)
                {
                    BvhWriter.Write(dialog.FileName, this.recorder.BodyFrame.Motion);

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
    }
}
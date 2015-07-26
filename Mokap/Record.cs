using Microsoft.Win32;
using Mokap.Bvh;
using System;
using System.Windows;

namespace Mokap
{
    class Record : MainWindow.State
    {
        private Recorder recorder;

        public Record(MainWindow mainWindow)
            : base(mainWindow)
        {
            mainWindow.RecordButton.Content = "Stop Record";
            mainWindow.RecordButton.IsEnabled = true;

            this.recorder = new Recorder();
            this.recorder.Start();

            mainWindow.ColorCamera.Source = this.recorder.ColorFrame.Bitmap;
            mainWindow.DepthCamera.Source = this.recorder.DepthFrame.Bitmap;
            mainWindow.BodyCamera.Source = this.recorder.BodyFrame.Bitmap;
        }

        protected override void DisposeManaged()
        {
            MainWindow.ColorCamera.Source = null;
            MainWindow.DepthCamera.Source = null;
            MainWindow.BodyCamera.Source = null;

            SafeDispose(ref this.recorder);

            base.DisposeManaged();
        }

        public override void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            base.RecordButton_Click(sender, e);

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
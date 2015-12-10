using Microsoft.Win32;
using Mokap.Bvh;
using Mokap.Controls;
using Mokap.Data;
using Mokap.Properties;
using System;
using System.Windows;

namespace Mokap.States
{
    class Replaying : MainWindow.State
    {
        private string filename;

        private Replayer replayer;

        private BodyViewport bodyViewport;

        public Replaying(MainWindow mainWindow, string filename, Replayer replayer)
            : base(mainWindow)
        {
            bodyViewport = new BodyViewport(mainWindow.BodyViewport);

            this.filename = filename;
            this.replayer = replayer;
            this.replayer.Start();

            replayer.BodyFrameUpdated += Recorder_BodyFrameUpdated;

            mainWindow.ReplayButton.Content = Resources.StopReplay;
            mainWindow.ReplayButton.Click += ReplayButton_Click;

            mainWindow.ExportAsBvhButton.Click += ExportAsBvhButton_Click;
        }

        protected override void DisposeManaged()
        {
            replayer.BodyFrameUpdated -= Recorder_BodyFrameUpdated;
            MainWindow.ReplayButton.Click -= ReplayButton_Click;

            SafeDispose(ref replayer);
            SafeDispose(ref bodyViewport);

            base.DisposeManaged();
        }

        private void Recorder_BodyFrameUpdated(object sender, BodyFrameUpdatedEventArgs e)
        {
            bodyViewport.Update(e.Frame);
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            Become(new Idle(MainWindow));
        }

        private void ExportAsBvhButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                FileName = string.Format("Mokap_{0}.bvh", DateTime.Now.ToString("yyyyMMdd_HHmmss")),
                Filter = "BVH Files|*.bvh",
            };

            if (dialog.ShowDialog() == true)
            {
                BvhWriter.Write(dialog.FileName, filename);
            }
            else
            {
                logger.Trace("User cancelled export bvh file");
            }
        }
    }
}

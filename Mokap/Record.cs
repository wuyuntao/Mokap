using Microsoft.Win32;
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

            var dialog = new SaveFileDialog()
            {
                FileName = "Mokap.bvh",
                Filter = "Biovision Hierarchy Files|*.bvh",
            };

            if (dialog.ShowDialog() == true)
            {
                logger.Trace("Record to file {0}", dialog.FileName);
            }

            Become(new Idle(MainWindow));
        }
    }
}
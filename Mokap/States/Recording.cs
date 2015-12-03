using Mokap.Data;
using Mokap.Properties;
using System.Windows;

namespace Mokap.States
{
    class Recording : Capturing
    {
        private Recorder recorder;

        public Recording(MainWindow mainWindow, string filename)
            : base(mainWindow, Metadata.CreateFromKinectSensor())
        {
            mainWindow.RecordButton.Content = Resources.StopRecording;
            mainWindow.RecordButton.IsEnabled = true;
            mainWindow.RecordButton.Click += RecordButton_Click;

            recorder = new Recorder(filename, Metadata);
            recorder.Start();

            recorder.BodyFrameUpdated += Recorder_BodyFrameUpdated;
            recorder.ColorFrameUpdated += Recorder_ColorFrameUpdated;
            recorder.DepthFrameUpdated += Recorder_DepthFrameUpdated;
        }

        protected override void DisposeManaged()
        {
            MainWindow.RecordButton.Click -= RecordButton_Click;

            SafeDispose(ref recorder);

            base.DisposeManaged();
        }

        private void Recorder_BodyFrameUpdated(object sender, BodyFrameUpdatedEventArgs e)
        {
            BodyCamera.Update(e.Frame);
        }

        private void Recorder_ColorFrameUpdated(object sender, ColorFrameUpdatedEventArgs e)
        {
            ColorCamera.Update(e.Frame);
        }

        private void Recorder_DepthFrameUpdated(object sender, DepthFrameUpdatedEventArgs e)
        {
            DepthCamera.Update(e.Frame);
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            recorder.Stop();

            Become(new Idle(MainWindow));
        }
    }
}
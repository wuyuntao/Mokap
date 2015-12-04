using Mokap.Controls;
using Mokap.Data;
using Mokap.Properties;
using System.Windows;

namespace Mokap.States
{
    class Recording : MainWindow.State
    {
        private Recorder recorder;

        private BodyCamera bodyCamera;

        private ColorCamera colorCamera;

        private DepthCamera depthCamera;

        public Recording(MainWindow mainWindow, Recorder recorder)
            : base(mainWindow)
        {
            mainWindow.RecordButton.Content = Resources.StopRecording;
            mainWindow.RecordButton.IsEnabled = true;
            mainWindow.RecordButton.Click += RecordButton_Click;

            bodyCamera = new BodyCamera(mainWindow.BodyCamera, recorder.Metadata.DepthFrameWidth, recorder.Metadata.DepthFrameHeight);
            colorCamera = new ColorCamera(mainWindow.ColorCamera, recorder.Metadata.ColorFrameWidth, recorder.Metadata.ColorFrameHeight);
            depthCamera = new DepthCamera(mainWindow.DepthCamera, recorder.Metadata.DepthFrameWidth, recorder.Metadata.DepthFrameHeight);

            this.recorder = recorder;
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
            bodyCamera.Update(e.Frame);
        }

        private void Recorder_ColorFrameUpdated(object sender, ColorFrameUpdatedEventArgs e)
        {
            colorCamera.Update(e.Frame);
        }

        private void Recorder_DepthFrameUpdated(object sender, DepthFrameUpdatedEventArgs e)
        {
            depthCamera.Update(e.Frame);
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            recorder.Stop();

            Become(new Idle(MainWindow));
        }
    }
}
using Mokap.Properties;
using System.Windows;
using Mokap.Data;
using System;
using Mokap.Controls;

namespace Mokap.States
{
    class Replaying : MainWindow.State
    {
        private Replayer replayer;

        private BodyViewport bodyViewport;

        public Replaying(MainWindow mainWindow, Replayer replayer)
            : base(mainWindow)
        {
            bodyViewport = new BodyViewport(mainWindow.BodyViewport);

            this.replayer = replayer;
            this.replayer.Start();

            replayer.BodyFrameUpdated += Recorder_BodyFrameUpdated;

            mainWindow.ReplayButton.Content = Resources.StopRecording;
            mainWindow.ReplayButton.Click += ReplayButton_Click;
        }

        protected override void DisposeManaged()
        {
            MainWindow.ReplayButton.Click -= ReplayButton_Click;

            SafeDispose(ref replayer);

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
    }
}

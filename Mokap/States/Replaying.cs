using Mokap.Properties;
using System.Windows;

namespace Mokap.States
{
    class Replaying : Capturing
    {
        private Replayer replayer;

        public Replaying(MainWindow mainWindow, Replayer replayer)
            : base(mainWindow, replayer.Metadata)
        {
            mainWindow.ReplayButton.Content = Resources.StopRecording;
            mainWindow.ReplayButton.IsEnabled = true;
            mainWindow.ReplayButton.Click += ReplayButton_Click;

            this.replayer = replayer;
            this.replayer.Start();
        }

        protected override void DisposeManaged()
        {
            MainWindow.ReplayButton.Click += ReplayButton_Click;

            SafeDispose(ref replayer);

            base.DisposeManaged();
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            Become(new Idle(MainWindow));
        }
    }
}

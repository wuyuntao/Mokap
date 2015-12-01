using Microsoft.Kinect;
using System.Windows;

namespace Mokap.States
{
    class Idle : MainWindow.State
    {
        public Idle(MainWindow mainWindow)
            : base(mainWindow)
        {
            mainWindow.RecordButton.Content = "Start Record";
            mainWindow.RecordButton.IsEnabled = true;
            mainWindow.RecordButton.Click += RecordButton_Click;

            mainWindow.ReplayButton.Content = "Start Replay";
            mainWindow.ReplayButton.IsEnabled = true;
            mainWindow.ReplayButton.Click += ReplayButton_Click;
        }

        protected override void DisposeManaged()
        {
            MainWindow.RecordButton.Click -= RecordButton_Click;
            MainWindow.RecordButton.IsEnabled = false;

            MainWindow.ReplayButton.Click -= ReplayButton_Click;
            MainWindow.ReplayButton.IsEnabled = false;

            base.DisposeManaged();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Check if Kinect sensor is ready
            var sensor = KinectSensor.GetDefault();
            if (sensor != null && sensor.IsAvailable)
            {
                Become(new Record(MainWindow));
            }
            else
            {
                MessageBox.Show("Kinect sensor is not connected", "Mokap");
            }
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            // Try load raw data
            Become(new Replay(MainWindow));
        }
    }
}
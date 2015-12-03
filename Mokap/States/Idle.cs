using Microsoft.Kinect;
using Microsoft.Win32;
using Mokap.Properties;
using System;
using System.Windows;

namespace Mokap.States
{
    class Idle : MainWindow.State
    {
        public Idle(MainWindow mainWindow)
            : base(mainWindow)
        {
            mainWindow.RecordButton.Content = Resources.StartRecording;
            mainWindow.RecordButton.IsEnabled = true;
            mainWindow.RecordButton.Click += RecordButton_Click;

            mainWindow.ReplayButton.Content = Resources.StartReplay;
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
            var dialog = new SaveFileDialog()
            {
                FileName = string.Format("Mokap_{0}.mkp", DateTime.Now.ToString("yyyyMMdd_HHmmss")),
                Filter = "Mokap Record Files|*.mkp",
            };

            if (dialog.ShowDialog() == true)
            {
                var recorder = new Recorder(dialog.FileName);

                // TODO Check if recorder can start

                Become(new Recording(MainWindow, recorder));
            }
            else
            {
                logger.Trace("User cancelled saving mkp file");
            }
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Mokap Record Files|*.mkp",
            };

            if (dialog.ShowDialog() == true)
            {
                var replayer = new Replayer(dialog.FileName);

                // TODO Check if replayer can start

                Become(new Replaying(MainWindow, replayer));
            }
            else
            {
                logger.Trace("User cancelled load mkp file");
            }
        }
    }
}
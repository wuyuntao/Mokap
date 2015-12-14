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
            //Bvh.BvhWriter2.Write(@"D:\Downloads\Mokap_20151203_213032.bvh", @"D:\Downloads\Mokap_20151203_213032.mkp");

            mainWindow.RecordButton.Content = Resources.StartRecording;
#if NO_KINECT
            mainWindow.RecordButton.IsEnabled = false;
#else
            mainWindow.RecordButton.IsEnabled = true;
#endif
            mainWindow.RecordButton.Click += RecordButton_Click;
            mainWindow.ReplayButton.Click += ReplayButton_Click;
        }

        protected override void DisposeManaged()
        {
            MainWindow.RecordButton.Click -= RecordButton_Click;
            MainWindow.ReplayButton.Click -= ReplayButton_Click;

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
                var recorder = new Recorder(dialog.FileName, MainWindow.Dispatcher);

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
                var replayer = new Replayer(dialog.FileName, MainWindow.Dispatcher);

                // TODO Check if replayer can start

                Become(new Replaying(MainWindow, dialog.FileName, replayer));
            }
            else
            {
                logger.Trace("User cancelled load mkp file");
            }
        }
    }
}
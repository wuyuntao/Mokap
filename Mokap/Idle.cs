using Mokap.Bvh;
using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows;

namespace Mokap
{
    class Idle : MainWindow.State
    {
        public Idle(MainWindow mainWindow)
            : base(mainWindow)
        {
            mainWindow.RecordButton.Content = "Start Record";
            mainWindow.RecordButton.IsEnabled = true;
        }

        public override void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            base.RecordButton_Click(sender, e);

            Become(new Record(MainWindow));
        }

        public override void TestButton_Click(object sender, RoutedEventArgs e)
        {
            base.TestButton_Click(sender, e);

            TestBvhWriter();

            MessageBox.Show("Done", "Mokap");
        }

        private void TestBvhWriter()
        {
            var dataFilename = Path.Combine(Environment.CurrentDirectory, "Resources/BodyFrameData.csv");
            if (!File.Exists(dataFilename))
            {
                logger.Warn("Data file not exist: {0}", dataFilename);
                return;
            }

            Motion motion = null;
            var bodies = CsvBodyAdapter.ParseFromCsvFile(dataFilename);

            foreach (var body in bodies)
            {
                if (motion == null)
                {
                    motion = new Motion(body);
                }
                else
                {
                    motion.AppendFrame(body);
                }
            }

            var bvhFilename = Path.Combine(Environment.CurrentDirectory, "TestBvhMotion.bvh");
            BvhWriter.Write(bvhFilename, motion);
        }
    }
}
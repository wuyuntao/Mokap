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

            var motion = new Motion();
            var bodies = CsvBodyAdapter.ParseFromCsvFile(dataFilename);

            foreach (var body in bodies)
            {
                if (!motion.HasSkeleton)
                {
                    motion.CreateSkeleton(body);
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
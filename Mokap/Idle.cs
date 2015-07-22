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
    }
}
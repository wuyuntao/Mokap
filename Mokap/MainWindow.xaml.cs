using Microsoft.Kinect;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mokap
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private State state;

        #region State

        internal abstract class State : Disposable
        {
            protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

            private MainWindow mainWindow;

            protected State(MainWindow mainWindow)
            {
                this.mainWindow = mainWindow;
            }

            protected virtual void Become(State nextState)
            {
                if (this.mainWindow.state != null)
                {
                    this.mainWindow.state.Dispose();
                }

                this.mainWindow.state = nextState;

                if (nextState != null)
                    logger.Trace("State changed from {0} to {1}", GetType().Name, nextState.GetType().Name);
                else
                    logger.Trace("State stoped from {0}", GetType().Name);
            }

            #region UI Events

            public virtual void RecordButton_Click(object sender, RoutedEventArgs e)
            {
                logger.Trace("RecordButton is clicked");
            }

            #endregion

            #region Properties

            protected MainWindow MainWindow
            {
                get { return this.mainWindow; }
            }

            #endregion
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region UI Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logger.Trace("Loaded");

            this.state = new Idle(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logger.Trace("Closing");

            if (state != null)
            {
                state.Dispose();
                state = null;
            }

            // Always try closing Kinect sensor when window is closing
            var sensor = KinectSensor.GetDefault();
            if (sensor != null && sensor.IsOpen)
                sensor.Close();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.state != null)
                this.state.RecordButton_Click(sender, e);
        }

        #endregion
    }
}

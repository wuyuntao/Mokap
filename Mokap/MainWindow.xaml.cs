using Microsoft.Kinect;
using Mokap.States;
using NLog;
using System.Windows;

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

            protected void Become(State nextState)
            {
                if (mainWindow.state != null)
                {
                    mainWindow.state.Dispose();
                }

                mainWindow.state = nextState;

                if (nextState != null)
                    logger.Trace("State changed from {0} to {1}", GetType().Name, nextState.GetType().Name);
                else
                    logger.Trace("State stoped from {0}", GetType().Name);
            }

            #region Properties

            protected MainWindow MainWindow
            {
                get { return mainWindow; }
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

            state = new Idle(this);
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
            if (!KinectHelper.IsSensorAvailable())
            {
                var sensor = KinectSensor.GetDefault();
                if (sensor != null && sensor.IsOpen)
                    sensor.Close();
            }
        }

        #endregion
    }
}

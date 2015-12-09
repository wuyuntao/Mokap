using Mokap.Data;
using Mokap.Properties;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace Mokap
{
    sealed class Replayer : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<BodyFrameUpdatedEventArgs> BodyFrameUpdated;

        private RecordReader reader;

        private bool started;

        private Stopwatch stopwatch;

        private Dispatcher dispatcher;

        public Replayer(string filename, Dispatcher dispatcher)
        {
            reader = new RecordReader(filename);

            this.dispatcher = dispatcher;
        }

        protected override void DisposeManaged()
        {
            Stop();

            reader.Dispose();

            base.DisposeManaged();
        }

        public void Start()
        {
            if (!started)
            {
                started = true;
                stopwatch = Stopwatch.StartNew();

                ThreadPool.QueueUserWorkItem(WorkThread);
            }
        }

        public void Stop()
        {
            if (!started)
            {
                started = false;
                stopwatch.Stop();
            }
        }

        #region Work Thread

        private void WorkThread(object state)
        {
            while (started)
            {
                var message = reader.ReadNextMessage();
                if (message == null)
                {
                    break;
                }
                else if (message is BodyFrameData)
                {
                    var frame = (BodyFrameData)message;
                    if (frame.RelativeTime > stopwatch.Elapsed)
                        Thread.Sleep(frame.RelativeTime - stopwatch.Elapsed);

                    if (BodyFrameUpdated != null)
                    {
                        dispatcher.Invoke(() =>
                        {
                            if (started && BodyFrameUpdated != null)
                                BodyFrameUpdated(this, new BodyFrameUpdatedEventArgs(frame));
                        });
                    }
                }
                else
                {
                    throw new InvalidDataException(Resources.IllegalRecordDataFormat);
                }
            }
        }

        #endregion
    }
}

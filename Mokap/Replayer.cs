using FlatBuffers.Schema;
using Mokap.Data;
using Mokap.Properties;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using BodyFrameDataMsg = Mokap.Schemas.RecorderMessages.BodyFrameData;
using MessageIds = Mokap.Schemas.RecorderMessages.MessageIds;
using MetadataMsg = Mokap.Schemas.RecorderMessages.Metadata;

namespace Mokap
{
    sealed class Replayer : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const int ReadBufferSize = 8192;

        public event EventHandler<BodyFrameUpdatedEventArgs> BodyFrameUpdated;

        private Metadata metadata;

        private FileStream fileStream;

        private MessageQueue messages;

        private bool started;

        private Stopwatch stopwatch;

        private Dispatcher dispatcher;

        public Replayer(string filename, Dispatcher dispatcher)
        {
            fileStream = new FileStream(filename, FileMode.Open);

            var schema = new MessageSchema();
            schema.Register(MessageIds.Metadata, MetadataMsg.GetRootAsMetadata);
            schema.Register(MessageIds.BodyFrameData, BodyFrameDataMsg.GetRootAsBodyFrameData);

            messages = new MessageQueue(schema);

            ReadMetadata();

            this.dispatcher = dispatcher;
        }

        protected override void DisposeManaged()
        {
            Stop();

            fileStream.Close();

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
                var message = ReadNextMessage();
                if (message == null)
                {
                    break;
                }

                var messageId = (MessageIds)message.Id;
                switch (messageId)
                {
                    case MessageIds.BodyFrameData:
                        {
                            var frame = BodyFrameData.Deserialize((BodyFrameDataMsg)message.Body);
                            SleepUntil(frame.RelativeTime);

                            if (BodyFrameUpdated != null)
                            {
                                dispatcher.Invoke(() => BodyFrameUpdated(this, new BodyFrameUpdatedEventArgs(frame)));
                            }
                            break;
                        }

                    default:
                        throw new InvalidDataException(Resources.IllegalRecordDataFormat);
                }
            }
        }

        private void SleepUntil(TimeSpan time)
        {
            if (time > stopwatch.Elapsed)
                Thread.Sleep(time - stopwatch.Elapsed);
        }

        #endregion

        private void ReadMetadata()
        {
            var message = ReadNextMessage();
            if (message != null && message.Id == (int)MessageIds.Metadata)
                metadata = Metadata.Deserialize((MetadataMsg)message.Body);
            else
                throw new InvalidDataException(Resources.IllegalRecordDataFormat);
        }

        private Message ReadNextMessage()
        {
            Message msg = null;

            for (var eof = false; msg == null && !eof; msg = messages.Dequeue())
            {
                var bytes = new byte[ReadBufferSize];
                var readSize = fileStream.Read(bytes, 0, bytes.Length);
                if (readSize > 0)
                    messages.Enqueue(bytes, 0, readSize);
                else
                    eof = true;
            }

            return msg;
        }

        public Metadata Metadata
        {
            get { return metadata; }
        }
    }
}

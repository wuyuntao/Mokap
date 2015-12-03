using FlatBuffers.Schema;
using Mokap.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageIds = Mokap.Schemas.RecorderMessages.MessageIds;
using BodyFrameDataMsg = Mokap.Schemas.RecorderMessages.BodyFrameData;
using ColorFrameDataMsg = Mokap.Schemas.RecorderMessages.ColorFrameData;
using DepthFrameDataMsg = Mokap.Schemas.RecorderMessages.DepthFrameData;
using MetadataMsg = Mokap.Schemas.RecorderMessages.Metadata;
using Mokap.Properties;

namespace Mokap
{
    sealed class Replayer : Disposable
    {
        private const int ReadBufferSize = 8192;

        public event EventHandler<BodyFrameUpdatedEventArgs> BodyFrameUpdated;

        public event EventHandler<ColorFrameUpdatedEventArgs> ColorFrameUpdated;

        public event EventHandler<DepthFrameUpdatedEventArgs> DepthFrameUpdated;

        private Metadata metadata;

        private FileStream fileStream;

        private MessageQueue messages;

        public Replayer(string filename)
        {
            fileStream = new FileStream(filename, FileMode.Open);

            var schema = new MessageSchema();
            schema.Register(MessageIds.Metadata, MetadataMsg.GetRootAsMetadata);
            schema.Register(MessageIds.BodyFrameData, BodyFrameDataMsg.GetRootAsBodyFrameData);
            schema.Register(MessageIds.ColorFrameData, ColorFrameDataMsg.GetRootAsColorFrameData);
            schema.Register(MessageIds.DepthFrameData, DepthFrameDataMsg.GetRootAsDepthFrameData);

            messages = new MessageQueue(schema);

            ReadMetadata();
        }

        protected override void DisposeManaged()
        {
            Stop();

            fileStream.Close();

            base.DisposeManaged();
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

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

            for (var eof = false; msg != null || eof; msg = messages.Dequeue())
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

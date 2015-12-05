using FlatBuffers.Schema;
using Mokap.Data;
using Mokap.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using BodyFrameDataMsg = Mokap.Schemas.RecorderMessages.BodyFrameData;
using MessageIds = Mokap.Schemas.RecorderMessages.MessageIds;
using MetadataMsg = Mokap.Schemas.RecorderMessages.Metadata;

namespace Mokap
{
    sealed class RecordReader : Disposable
    {
        private const int ReadBufferSize = 8192;

        private FileStream fileStream;

        private MessageQueue messages;

        private Metadata metadata;

        public RecordReader(string filename)
        {
            fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            var schema = new MessageSchema();
            schema.Register(MessageIds.Metadata, MetadataMsg.GetRootAsMetadata);
            schema.Register(MessageIds.BodyFrameData, BodyFrameDataMsg.GetRootAsBodyFrameData);

            messages = new MessageQueue(schema);

            metadata = ReadNextMessage() as Metadata;
            if (metadata == null)
                throw new InvalidDataException(Resources.IllegalRecordDataFormat);
        }

        protected override void DisposeManaged()
        {
            fileStream.Close();

            base.DisposeManaged();
        }

        public object ReadNextMessage()
        {
            Message msg = null;

            for (var eof = false; msg == null && !eof; msg = messages.Dequeue())
            {
                var bytes = new byte[ReadBufferSize];
                try
                {
                    var readSize = fileStream.Read(bytes, 0, bytes.Length);
                    if (readSize > 0)
                        messages.Enqueue(bytes, 0, readSize);
                    else
                        eof = true;
                }
                catch (ObjectDisposedException)
                {
                    eof = true;
                }
            }

            if (msg != null)
            {
                var messageId = (MessageIds)msg.Id;
                switch (messageId)
                {
                    case MessageIds.Metadata:
                        return Metadata.Deserialize((MetadataMsg)msg.Body);

                    case MessageIds.BodyFrameData:
                        return BodyFrameData.Deserialize((BodyFrameDataMsg)msg.Body);

                    default:
                        throw new InvalidDataException(Resources.IllegalRecordDataFormat);
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> ReadAllMessages()
        {
            for (var msg = ReadNextMessage(); msg != null; msg = ReadNextMessage())
            {
                yield return msg;
            }
        }

        public Metadata Metadata
        {
            get { return metadata; }
        }
    }
}
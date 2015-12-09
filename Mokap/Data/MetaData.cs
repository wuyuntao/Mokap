using FlatBuffers;
using FlatBuffers.Schema;
using Mokap.Schemas.RecorderMessages;
using System;
using MetadataMsg = Mokap.Schemas.RecorderMessages.Metadata;

namespace Mokap.Data
{
    [Serializable]
    sealed class Metadata
    {
        public int ColorFrameWidth;

        public int ColorFrameHeight;

        public int DepthFrameWidth;

        public int DepthFrameHeight;

        public static Metadata Deserialize(MetadataMsg message)
        {
            return new Metadata()
            {
                ColorFrameWidth = message.ColorFrameWidth,
                ColorFrameHeight = message.ColorFrameHeight,
                DepthFrameWidth = message.DepthFrameWidth,
                DepthFrameHeight = message.DepthFrameHeight,
            };
        }

        public byte[] Serialize()
        {
            var fbb = new FlatBufferBuilder(1024);
            var msg = MetadataMsg.CreateMetadata(fbb,
                    ColorFrameWidth, ColorFrameHeight,
                    DepthFrameWidth, DepthFrameHeight);
            fbb.Finish(msg.Value);

            return fbb.ToProtocolMessage(MessageIds.Metadata);
        }
    }
}
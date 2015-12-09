using System;

namespace Mokap.Data
{
    [Serializable]
    sealed class ColorFrameData
    {
        public TimeSpan RelativeTime;

        public int Width;

        public int Height;

        public byte[] Data;
    }
}
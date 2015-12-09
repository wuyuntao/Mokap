using System;

namespace Mokap.Data
{
    [Serializable]
    sealed class DepthFrameData
    {
        public TimeSpan RelativeTime;

        public int Width;

        public int Height;

        public ushort[] Data;

        public ushort MinReliableDistance;

        public ushort MaxReliableDistance;
    }
}

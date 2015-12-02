using System;

namespace Mokap.Data
{
    [Serializable]
    class DepthFrameData
    {
        public TimeSpan RelativeTime;

        public int Width;

        public int Height;

        public ushort[] Data;

        public ushort MinReliableDistance;

        public ushort MaxReliableDistance;
    }
}

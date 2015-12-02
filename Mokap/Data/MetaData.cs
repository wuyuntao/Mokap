using Microsoft.Kinect;
using System;

namespace Mokap.Data
{
    [Serializable]
    sealed class MetaData
    {
        public int ColorFrameWidth;

        public int ColorFrameHeight;

        public int DepthFrameWidth;

        public int DepthFrameHeight;
    }
}

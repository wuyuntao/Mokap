using System;

namespace Mokap.Data
{
    sealed class DepthFrameUpdatedEventArgs : EventArgs
    {
        public DepthFrameUpdatedEventArgs(DepthFrameData frame)
        {
            Frame = frame;
        }

        public DepthFrameData Frame { get; private set; }
    }
}

using System;

namespace Mokap.Data
{
    sealed class ColorFrameUpdatedEventArgs : EventArgs
    {
        public ColorFrameUpdatedEventArgs(ColorFrameData frame)
        {
            Frame = frame;
        }

        public ColorFrameData Frame { get; private set; }
    }
}

using System;

namespace Mokap.Data
{
    sealed class BodyFrameUpdatedEventArgs : EventArgs
    {
        public BodyFrameUpdatedEventArgs(BodyFrameData frame)
        {
            Frame = frame;
        }

        public BodyFrameData Frame { get; private set; }
    }
}

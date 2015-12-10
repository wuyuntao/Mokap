using System;
using System.Collections.Generic;

namespace Mokap.Data
{
    [Serializable]
    sealed class MotionData
    {
        public MotionBodyData Body;

        public List<MotionFrameData> Frames = new List<MotionFrameData>();

        public static MotionData CreateData(BodyFrameData bodyFrame)
        {
            var data = new MotionData()
            {
                Body = MotionBodyData.CreateData(bodyFrame),
            };

            data.AppendFrame(bodyFrame);

            return data;
        }

        public void AppendFrame(BodyFrameData bodyFrame)
        {
            Frames.Add(MotionFrameData.CreateData(bodyFrame, Body));
        }
    }
}

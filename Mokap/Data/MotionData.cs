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

        public int FrameCount
        {
            get { return Frames.Count; }
        }

        public TimeSpan TotalTime
        {
            get
            {
                if (Frames.Count <= 1)
                {
                    return TimeSpan.Zero;
                }

                var startTime = Frames[0].RelativeTime;
                var endTime = Frames[Frames.Count - 1].RelativeTime;

                return endTime - startTime;
            }
        }
    }
}

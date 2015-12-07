using GlmSharp;

namespace BvhExporter.Data
{
    static class MotionSet
    {
        public static Motion Simple1
        {
            get
            {
                var motion = new Motion();
                motion.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0, 0, 1),
                    Rotation = new quat(0, 0, 0, 1),
                    RotationAngles = new vec3(0, 0, 0),
                });

                // 1
                var frame = new Frame() { Time = 1 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0, 0, 1),
                    Rotation = new quat(0, 0, 0, 1),
                    RotationAngles = new vec3(0, 0, 0),
                });
                motion.Frames.Add(frame);

                // 10
                frame = new Frame() { Time = 10 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0.604f, 0, 0.797f),
                    Rotation = new quat(0, 0, -0.318f, 0.948f),
                    RotationAngles = new vec3(0, 37.131336f, 0),
                });
                motion.Frames.Add(frame);

                // 20
                frame = new Frame() { Time = 20 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0.604f, 0.432f, 0.67f),
                    Rotation = new quat(0.267f, 0.09f, -0.305f, 0.909f),
                    RotationAngles = new vec3(32.776763f, 37.131340f, 0),
                });
                motion.Frames.Add(frame);

                return motion;
            }
        }
    }
}

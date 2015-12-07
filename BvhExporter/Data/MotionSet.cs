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
                    RotationAngles = new vec3(0, -0, 0),
                });

                // 1
                var frame = new Frame() { Time = 1 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0, 0, 1),
                    Rotation = new quat(0, 0, 0, 1),
                    RotationAngles = new vec3(0, -0, 0),
                });
                motion.Frames.Add(frame);

                // 10
                frame = new Frame() { Time = 10 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0.561f, 0, 0.828f),
                    Rotation = new quat(0, 0, -0.294f, 0.956f),
                    RotationAngles = new vec3(0, 0, -34.189f),
                });
                motion.Frames.Add(frame);

                // 20
                frame = new Frame() { Time = 20 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0.561f, 0.55f, 0.619f),
                    Rotation = new quat(0.34f, 0.105f, -0.274f, 0.893f),
                    RotationAngles = new vec3(36.389f, 21.973f, -26.813f),
                });
                motion.Frames.Add(frame);

                return motion;
            }
        }
    }
}

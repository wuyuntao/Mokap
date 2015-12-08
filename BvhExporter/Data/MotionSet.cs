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

        public static Motion Spine
        {
            get
            {
                var motion = new Motion();
                motion.Bones.Add(new Bone()
                {
                    Name = "SpineBase",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0, 0.3337337f, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0, 0.3337337f, 0),
                    TailPos = new vec3(0, 0.581777f, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0, 0.581777f, 0),
                    TailPos = new vec3(0, 0.66476926f, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0, 0.581777f, 0),
                    TailPos = new vec3(-0.1818914f, 0.581777f, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new vec3(-0.1818914f, 0.581777f, 0),
                    TailPos = new vec3(-0.4112974f, 0.581777f, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0, 0.581777f, 0),
                    TailPos = new vec3(0.1818914f, 0.581777f, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new vec3(0.1818914f, 0.581777f, 0),
                    TailPos = new vec3(0.4112974f, 0.581777f, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(-0.085884925f, 0, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new vec3(-0.085884925f, 0, 0),
                    TailPos = new vec3(-0.085884925f, -0.4331289f, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0, 0, 0),
                    TailPos = new vec3(0.085884925f, 0, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new vec3(0.085884925f, 0, 0),
                    TailPos = new vec3(0.085884925f, -0.4331289f, 0),
                });

                // 1
                var frame = new Frame() { Time = 1 };
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineBase",
                    HeadPos = new vec3(0.0113890832290053f, -0.44517907500267f, 2.30070400238037f),
                    TailPos = new vec3(0.0218616798520088f, -0.145339205861092f, 2.44687032699585f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.0218616798520088f, -0.145339205861092f, 2.44687032699585f),
                    TailPos = new vec3(0.0291184559464455f, 0.0806832611560822f, 2.54878520965576f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0.0291184559464455f, 0.0806832611560822f, 2.54878520965576f),
                    TailPos = new vec3(0.0315756686031818f, 0.156884253025055f, 2.5815737247467f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0.0291184559464455f, 0.0806832611560822f, 2.54878520965576f),
                    TailPos = new vec3(-0.153398245573044f, 0.0435472913086414f, 2.51644325256348f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new vec3(-0.153398245573044f, 0.0435472913086414f, 2.51644325256348f),
                    TailPos = new vec3(-0.376345813274384f, 0.00909638404846191f, 2.51425409317017f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0.0291184559464455f, 0.0806832611560822f, 2.54878520965576f),
                    TailPos = new vec3(0.193298190832138f, 0.0501831732690334f, 2.49732041358948f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new vec3(0.193298190832138f, 0.0501831732690334f, 2.49732041358948f),
                    TailPos = new vec3(0.406055271625519f, 0.074210949242115f, 2.40489220619202f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.0113890832290053f, -0.44517907500267f, 2.30070400238037f),
                    TailPos = new vec3(-0.0626806020736694f, -0.432130515575409f, 2.2574610710144f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new vec3(-0.0626806020736694f, -0.432130515575409f, 2.2574610710144f),
                    TailPos = new vec3(-0.108909033238888f, -0.869084060192108f, 2.151691198349f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.0113890832290053f, -0.44517907500267f, 2.30070400238037f),
                    TailPos = new vec3(0.0867793336510658f, -0.441536575555801f, 2.26158285140991f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new vec3(0.0867793336510658f, -0.441536575555801f, 2.26158285140991f),
                    TailPos = new vec3(0.111000396311283f, -0.829505443572998f, 2.11823010444641f),
                });

                motion.Frames.Add(frame);

                return motion;
            }
        }
    }
}

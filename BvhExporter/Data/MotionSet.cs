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

                #region T-Pose

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
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new vec3(-0.4112974f, 0.581777f, 0),
                    TailPos = new vec3(-0.6430283f, 0.581777f, 0),
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
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new vec3(0.4112974f, 0.581777f, 0),
                    TailPos = new vec3(0.6430283f, 0.581777f, 0),
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
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new vec3(-0.085884925f, -0.4331289f, 0),
                    TailPos = new vec3(-0.085884925f, -0.74170485f, 0),
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
                motion.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new vec3(0.085884925f, -0.4331289f, 0),
                    TailPos = new vec3(0.085884925f, -0.74170485f, 0),
                });

                #endregion

                #region Frame 1

                // cat 2015-12-08.log | tail -n+6000 | head-n31 | egrep "(Type|Time)"
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
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new vec3(-0.376345813274384f, 0.00909638404846191f, 2.51425409317017f),
                    TailPos = new vec3(-0.476233422756195f, 0.219231143593788f, 2.47339868545532f),
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
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new vec3(0.406055271625519f, 0.074210949242115f, 2.40489220619202f),
                    TailPos = new vec3(0.468577086925507f, 0.289391905069351f, 2.36716055870056f),
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
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new vec3(-0.108909033238888f, -0.869084060192108f, 2.151691198349f),
                    TailPos = new vec3(-0.133804127573967f, -1.17661082744598f, 2.12549781799316f),
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
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new vec3(0.111000396311283f, -0.829505443572998f, 2.11823010444641f),
                    TailPos = new vec3(0.145995542407036f, -1.13450086116791f, 2.13598132133484f),
                });

                motion.Frames.Add(frame);

                #endregion

                #region Frame 2

                // cat 2015-12-08.log | tail -n+3000 | head -n31 |egrep "(Type|Time)"
                frame = new Frame() { Time = 2 };
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineBase",
                    HeadPos = new vec3(0.132761403918266f, -0.516510426998138f, 2.49189376831055f),
                    TailPos = new vec3(0.176697865128517f, -0.195139482617378f, 2.60428690910339f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.176697865128517f, -0.195139482617378f, 2.60428690910339f),
                    TailPos = new vec3(0.211136922240257f, 0.0261364467442036f, 2.69899272918701f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0.211136922240257f, 0.0261364467442036f, 2.69899272918701f),
                    TailPos = new vec3(0.0315756686031818f, 0.156884253025055f, 2.5815737247467f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0.0384622178971767f, -0.0181573610752821f, 2.66484379768372f),
                    TailPos = new vec3(-0.153398245573044f, 0.0435472913086414f, 2.51644325256348f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new vec3(0.0384622178971767f, -0.0181573610752821f, 2.66484379768372f),
                    TailPos = new vec3(-0.00472871214151382f, -0.215796589851379f, 2.54035782814026f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new vec3(-0.00472871214151382f, -0.215796589851379f, 2.54035782814026f),
                    TailPos = new vec3(-0.119839549064636f, -0.296535551548004f, 2.25638437271118f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(0.211136922240257f, 0.0261364467442036f, 2.69899272918701f),
                    TailPos = new vec3(0.373187869787216f, -0.033208355307579f, 2.62990760803223f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new vec3(0.373187869787216f, -0.033208355307579f, 2.62990760803223f),
                    TailPos = new vec3(0.445780247449875f, -0.301416456699371f, 2.56785583496094f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new vec3(0.445780247449875f, -0.301416456699371f, 2.56785583496094f),
                    TailPos = new vec3(0.433257192373276f, -0.507047593593597f, 2.4624137878418f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.132761403918266f, -0.516510426998138f, 2.49189376831055f),
                    TailPos = new vec3(0.050908837467432f, -0.490241467952728f, 2.45252132415771f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new vec3(0.050908837467432f, -0.490241467952728f, 2.45252132415771f),
                    TailPos = new vec3(-0.126664042472839f, -0.213066712021828f, 2.21701836585999f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new vec3(-0.108909033238888f, -0.869084060192108f, 2.151691198349f),
                    TailPos = new vec3(-0.269881427288055f, -0.433430433273315f, 1.94808149337769f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.132761403918266f, -0.516510426998138f, 2.49189376831055f),
                    TailPos = new vec3(0.209280744194984f, -0.524286210536957f, 2.44902300834656f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new vec3(0.209280744194984f, -0.524286210536957f, 2.44902300834656f),
                    TailPos = new vec3(0.147121801972389f, -0.799345910549164f, 2.21965789794922f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new vec3(0.111000396311283f, -0.829505443572998f, 2.11823010444641f),
                    TailPos = new vec3(0.164478585124016f, -1.18087196350098f, 2.23274111747742f),
                });

                motion.Frames.Add(frame);

                #endregion

                #region Frame 3

                // RelativeTime=00:00:03.7347907
                frame = new Frame() { Time = 3 };
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineBase",
                    HeadPos = new vec3(0.000342677289154381f, -0.543801844120026f, 2.51458430290222f),
                    TailPos = new vec3(-0.0106440763920546f, -0.210145875811577f, 2.60062694549561f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(-0.0106440763920546f, -0.210145875811577f, 2.60062694549561f),
                    TailPos = new vec3(-0.0182632058858871f, 0.0342400632798672f, 2.65651178359985f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(-0.0182632058858871f, 0.0342400632798672f, 2.65651178359985f),
                    TailPos = new vec3(-0.0208033081144094f, 0.11417231708765f, 2.671715259552f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(-0.0182632058858871f, 0.0342400632798672f, 2.65651178359985f),
                    TailPos = new vec3(-0.199284851551056f, -0.0104948002845049f, 2.63554906845093f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new vec3(-0.199284851551056f, -0.0104948002845049f, 2.63554906845093f),
                    TailPos = new vec3(-0.262059062719345f, -0.290408134460449f, 2.50387072563171f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new vec3(-0.262059062719345f, -0.290408134460449f, 2.50387072563171f),
                    TailPos = new vec3(-0.271264493465424f, -0.505774080753326f, 2.41388559341431f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new vec3(-0.0182632058858871f, 0.0342400632798672f, 2.65651178359985f),
                    TailPos = new vec3(0.152254045009613f, -0.00279361847788095f, 2.64059948921204f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new vec3(0.152254045009613f, -0.00279361847788095f, 2.64059948921204f),
                    TailPos = new vec3(0.239086180925369f, -0.284051656723022f, 2.52987360954285f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new vec3(0.239086180925369f, -0.284051656723022f, 2.52987360954285f),
                    TailPos = new vec3(0.246769666671753f, -0.490002244710922f, 2.42526078224182f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.000342677289154381f, -0.543801844120026f, 2.51458430290222f),
                    TailPos = new vec3(-0.0779145509004593f, -0.532440721988678f, 2.47509932518005f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new vec3(-0.0779145509004593f, -0.532440721988678f, 2.47509932518005f),
                    TailPos = new vec3(-0.0924614071846008f, -0.777082800865173f, 2.21593022346497f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new vec3(-0.0924614071846008f, -0.777082800865173f, 2.21593022346497f),
                    TailPos = new vec3(-0.0831404402852058f, -1.15777671337128f, 2.22221517562866f),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new vec3(0.000342677289154381f, -0.543801844120026f, 2.51458430290222f),
                    TailPos = new vec3(0.0782122313976288f, -0.537419199943542f, 2.47317242622375f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new vec3(0.0782122313976288f, -0.537419199943542f, 2.47317242622375f),
                    TailPos = new vec3(0.135461509227753f, -0.830226182937622f, 2.27211403846741f),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new vec3(0.135461509227753f, -0.830226182937622f, 2.27211403846741f),
                    TailPos = new vec3(0.114599958062172f, -1.16649293899536f, 2.22849106788635f),
                });

                motion.Frames.Add(frame);

                #endregion

                return motion;
            }
        }
    }
}

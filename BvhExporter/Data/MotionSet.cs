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
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0, 0, 1),
                    Rotation = new dquat(0, 0, 0, 1),
                    RotationAngles = new dvec3(0, -0, 0),
                });

                // 1
                var frame = new Frame() { Time = 1 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0, 0, 1),
                    Rotation = new dquat(0, 0, 0, 1),
                    RotationAngles = new dvec3(0, -0, 0),
                });
                motion.Frames.Add(frame);

                // 10
                frame = new Frame() { Time = 10 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0.561, 0, 0.828),
                    Rotation = new dquat(0, 0, -0.294, 0.956),
                    RotationAngles = new dvec3(0, 0, -34.189),
                });
                motion.Frames.Add(frame);

                // 20
                frame = new Frame() { Time = 20 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Bone1",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0.561, 0.55, 0.619),
                    Rotation = new dquat(0.34, 0.105, -0.274, 0.893),
                    RotationAngles = new dvec3(36.389, 21.973, -26.813),
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
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0, 0.3337337, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0, 0.3337337, 0),
                    TailPos = new dvec3(0, 0.581777, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0, 0.581777, 0),
                    TailPos = new dvec3(0, 0.66476926, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0, 0.581777, 0),
                    TailPos = new dvec3(-0.1818914, 0.581777, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new dvec3(-0.1818914, 0.581777, 0),
                    TailPos = new dvec3(-0.4112974, 0.581777, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new dvec3(-0.4112974, 0.581777, 0),
                    TailPos = new dvec3(-0.6430283, 0.581777, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0, 0.581777, 0),
                    TailPos = new dvec3(0.1818914, 0.581777, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new dvec3(0.1818914, 0.581777, 0),
                    TailPos = new dvec3(0.4112974, 0.581777, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new dvec3(0.4112974, 0.581777, 0),
                    TailPos = new dvec3(0.6430283, 0.581777, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(-0.085884925, 0, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new dvec3(-0.085884925, 0, 0),
                    TailPos = new dvec3(-0.085884925, -0.4331289, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new dvec3(-0.085884925, -0.4331289, 0),
                    TailPos = new dvec3(-0.085884925, -0.74170485, 0),
                });

                motion.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0.085884925, 0, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new dvec3(0.085884925, 0, 0),
                    TailPos = new dvec3(0.085884925, -0.4331289, 0),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new dvec3(0.085884925, -0.4331289, 0),
                    TailPos = new dvec3(0.085884925, -0.74170485, 0),
                });

                #endregion

                #region Frame 1

                // cat 2015-12-08.log | tail -n+6000 | head-n31 | egrep "(Type|Time)"
                var frame = new Frame() { Time = 1 };
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineBase",
                    HeadPos = new dvec3(0.0113890832290053, -0.44517907500267, 2.30070400238037),
                    TailPos = new dvec3(0.0218616798520088, -0.145339205861092, 2.44687032699585),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.0218616798520088, -0.145339205861092, 2.44687032699585),
                    TailPos = new dvec3(0.0291184559464455, 0.0806832611560822, 2.54878520965576),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0.0291184559464455, 0.0806832611560822, 2.54878520965576),
                    TailPos = new dvec3(0.0315756686031818, 0.156884253025055, 2.5815737247467),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0.0291184559464455, 0.0806832611560822, 2.54878520965576),
                    TailPos = new dvec3(-0.153398245573044, 0.0435472913086414, 2.51644325256348),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new dvec3(-0.153398245573044, 0.0435472913086414, 2.51644325256348),
                    TailPos = new dvec3(-0.376345813274384, 0.00909638404846191, 2.51425409317017),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new dvec3(-0.376345813274384, 0.00909638404846191, 2.51425409317017),
                    TailPos = new dvec3(-0.476233422756195, 0.219231143593788, 2.47339868545532),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0.0291184559464455, 0.0806832611560822, 2.54878520965576),
                    TailPos = new dvec3(0.193298190832138, 0.0501831732690334, 2.49732041358948),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new dvec3(0.193298190832138, 0.0501831732690334, 2.49732041358948),
                    TailPos = new dvec3(0.406055271625519, 0.074210949242115, 2.40489220619202),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new dvec3(0.406055271625519, 0.074210949242115, 2.40489220619202),
                    TailPos = new dvec3(0.468577086925507, 0.289391905069351, 2.36716055870056),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.0113890832290053, -0.44517907500267, 2.30070400238037),
                    TailPos = new dvec3(-0.0626806020736694, -0.432130515575409, 2.2574610710144),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new dvec3(-0.0626806020736694, -0.432130515575409, 2.2574610710144),
                    TailPos = new dvec3(-0.108909033238888, -0.869084060192108, 2.151691198349),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new dvec3(-0.108909033238888, -0.869084060192108, 2.151691198349),
                    TailPos = new dvec3(-0.133804127573967, -1.17661082744598, 2.12549781799316),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.0113890832290053, -0.44517907500267, 2.30070400238037),
                    TailPos = new dvec3(0.0867793336510658, -0.441536575555801, 2.26158285140991),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new dvec3(0.0867793336510658, -0.441536575555801, 2.26158285140991),
                    TailPos = new dvec3(0.111000396311283, -0.829505443572998, 2.11823010444641),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new dvec3(0.111000396311283, -0.829505443572998, 2.11823010444641),
                    TailPos = new dvec3(0.145995542407036, -1.13450086116791, 2.13598132133484),
                });

                motion.Frames.Add(frame);

                #endregion

                #region Frame 2

                // cat 2015-12-08.log | tail -n+3000 | head -n31 |egrep "(Type|Time)"
                frame = new Frame() { Time = 2 };
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineBase",
                    HeadPos = new dvec3(0.132761403918266, -0.516510426998138, 2.49189376831055),
                    TailPos = new dvec3(0.176697865128517, -0.195139482617378, 2.60428690910339),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.176697865128517, -0.195139482617378, 2.60428690910339),
                    TailPos = new dvec3(0.211136922240257, 0.0261364467442036, 2.69899272918701),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0.211136922240257, 0.0261364467442036, 2.69899272918701),
                    TailPos = new dvec3(0.0315756686031818, 0.156884253025055, 2.5815737247467),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0.0384622178971767, -0.0181573610752821, 2.66484379768372),
                    TailPos = new dvec3(-0.153398245573044, 0.0435472913086414, 2.51644325256348),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new dvec3(0.0384622178971767, -0.0181573610752821, 2.66484379768372),
                    TailPos = new dvec3(-0.00472871214151382, -0.215796589851379, 2.54035782814026),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new dvec3(-0.00472871214151382, -0.215796589851379, 2.54035782814026),
                    TailPos = new dvec3(-0.119839549064636, -0.296535551548004, 2.25638437271118),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(0.211136922240257, 0.0261364467442036, 2.69899272918701),
                    TailPos = new dvec3(0.373187869787216, -0.033208355307579, 2.62990760803223),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new dvec3(0.373187869787216, -0.033208355307579, 2.62990760803223),
                    TailPos = new dvec3(0.445780247449875, -0.301416456699371, 2.56785583496094),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new dvec3(0.445780247449875, -0.301416456699371, 2.56785583496094),
                    TailPos = new dvec3(0.433257192373276, -0.507047593593597, 2.4624137878418),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.132761403918266, -0.516510426998138, 2.49189376831055),
                    TailPos = new dvec3(0.050908837467432, -0.490241467952728, 2.45252132415771),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new dvec3(0.050908837467432, -0.490241467952728, 2.45252132415771),
                    TailPos = new dvec3(-0.126664042472839, -0.213066712021828, 2.21701836585999),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new dvec3(-0.108909033238888, -0.869084060192108, 2.151691198349),
                    TailPos = new dvec3(-0.269881427288055, -0.433430433273315, 1.94808149337769),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.132761403918266, -0.516510426998138, 2.49189376831055),
                    TailPos = new dvec3(0.209280744194984, -0.524286210536957, 2.44902300834656),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new dvec3(0.209280744194984, -0.524286210536957, 2.44902300834656),
                    TailPos = new dvec3(0.147121801972389, -0.799345910549164, 2.21965789794922),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new dvec3(0.111000396311283, -0.829505443572998, 2.11823010444641),
                    TailPos = new dvec3(0.164478585124016, -1.18087196350098, 2.23274111747742),
                });

                motion.Frames.Add(frame);

                #endregion

                #region Frame 3

                // RelativeTime=00:00:03.7347907
                frame = new Frame() { Time = 3 };
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineBase",
                    HeadPos = new dvec3(0.000342677289154381, -0.543801844120026, 2.51458430290222),
                    TailPos = new dvec3(-0.0106440763920546, -0.210145875811577, 2.60062694549561),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineMid",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(-0.0106440763920546, -0.210145875811577, 2.60062694549561),
                    TailPos = new dvec3(-0.0182632058858871, 0.0342400632798672, 2.65651178359985),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "SpineShoulder",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(-0.0182632058858871, 0.0342400632798672, 2.65651178359985),
                    TailPos = new dvec3(-0.0208033081144094, 0.11417231708765, 2.671715259552),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderLeft",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(-0.0182632058858871, 0.0342400632798672, 2.65651178359985),
                    TailPos = new dvec3(-0.199284851551056, -0.0104948002845049, 2.63554906845093),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowLeft",
                    ParentName = "ShoulderLeft",
                    HeadPos = new dvec3(-0.199284851551056, -0.0104948002845049, 2.63554906845093),
                    TailPos = new dvec3(-0.262059062719345, -0.290408134460449, 2.50387072563171),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristLeft",
                    ParentName = "ElbowLeft",
                    HeadPos = new dvec3(-0.262059062719345, -0.290408134460449, 2.50387072563171),
                    TailPos = new dvec3(-0.271264493465424, -0.505774080753326, 2.41388559341431),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "ShoulderRight",
                    ParentName = "SpineMid",
                    HeadPos = new dvec3(-0.0182632058858871, 0.0342400632798672, 2.65651178359985),
                    TailPos = new dvec3(0.152254045009613, -0.00279361847788095, 2.64059948921204),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "ElbowRight",
                    ParentName = "ShoulderRight",
                    HeadPos = new dvec3(0.152254045009613, -0.00279361847788095, 2.64059948921204),
                    TailPos = new dvec3(0.239086180925369, -0.284051656723022, 2.52987360954285),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "WristRight",
                    ParentName = "ElbowRight",
                    HeadPos = new dvec3(0.239086180925369, -0.284051656723022, 2.52987360954285),
                    TailPos = new dvec3(0.246769666671753, -0.490002244710922, 2.42526078224182),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipLeft",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.000342677289154381, -0.543801844120026, 2.51458430290222),
                    TailPos = new dvec3(-0.0779145509004593, -0.532440721988678, 2.47509932518005),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeLeft",
                    ParentName = "HipLeft",
                    HeadPos = new dvec3(-0.0779145509004593, -0.532440721988678, 2.47509932518005),
                    TailPos = new dvec3(-0.0924614071846008, -0.777082800865173, 2.21593022346497),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleLeft",
                    ParentName = "KneeLeft",
                    HeadPos = new dvec3(-0.0924614071846008, -0.777082800865173, 2.21593022346497),
                    TailPos = new dvec3(-0.0831404402852058, -1.15777671337128, 2.22221517562866),
                });

                frame.Bones.Add(new Bone()
                {
                    Name = "HipRight",
                    ParentName = "SpineBase",
                    HeadPos = new dvec3(0.000342677289154381, -0.543801844120026, 2.51458430290222),
                    TailPos = new dvec3(0.0782122313976288, -0.537419199943542, 2.47317242622375),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "KneeRight",
                    ParentName = "HipRight",
                    HeadPos = new dvec3(0.0782122313976288, -0.537419199943542, 2.47317242622375),
                    TailPos = new dvec3(0.135461509227753, -0.830226182937622, 2.27211403846741),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "AnkleRight",
                    ParentName = "KneeRight",
                    HeadPos = new dvec3(0.135461509227753, -0.830226182937622, 2.27211403846741),
                    TailPos = new dvec3(0.114599958062172, -1.16649293899536, 2.22849106788635),
                });

                motion.Frames.Add(frame);

                #endregion

                return motion;
            }
        }

        public static Motion HalfSpine
        {
            get
            {
                var motion = new Motion();

                #region T-Pose

                motion.Bones.Add(new Bone()
                {
                    Name = "Chest",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0, 0, 0.25),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "Shoulder",
                    ParentName = "Chest",
                    HeadPos = new dvec3(0, 0, 0.25),
                    TailPos = new dvec3(-0.18, 0, 0.25),
                });
                motion.Bones.Add(new Bone()
                {
                    Name = "Arm",
                    ParentName = "Shoulder",
                    HeadPos = new dvec3(-0.18, 0, 0.25),
                    TailPos = new dvec3(-0.49, 0, 0.25),
                });

                #endregion

                #region Frame 1

                var frame = new Frame() { Time = 1 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Chest",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0, 0, 0.5),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Shoulder",
                    ParentName = "Chest",
                    HeadPos = new dvec3(0, 0, 0.5),
                    TailPos = new dvec3(-0.5, 0, 0.5),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Arm",
                    ParentName = "Shoulder",
                    HeadPos = new dvec3(-0.5, 0, 0.5),
                    TailPos = new dvec3(-1, 0, 0.5),
                });
                motion.Frames.Add(frame);

                #endregion

                #region Frame 2

                frame = new Frame() { Time = 2 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Chest",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(0, 0, 0.5),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Shoulder",
                    ParentName = "Chest",
                    HeadPos = new dvec3(0, 0, 0.5),
                    TailPos = new dvec3(-0.5, 0, 0.5),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Arm",
                    ParentName = "Shoulder",
                    HeadPos = new dvec3(-0.5, 0, 0.5),
                    TailPos = new dvec3(-0.5, 0, 0),
                });
                motion.Frames.Add(frame);

                #endregion

                #region Frame 3

                frame = new Frame() { Time = 3 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Chest",
                    HeadPos = new dvec3(-0, 0, 0),
                    TailPos = new dvec3(-0.1, 0.05, 0.25),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Shoulder",
                    ParentName = "Chest",
                    HeadPos = new dvec3(-0.1, 0.05, 0.25),
                    TailPos = new dvec3(-0.19, 0.03, 0.21),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Arm",
                    ParentName = "Shoulder",
                    HeadPos = new dvec3(-0.19, 0.03, 0.21),
                    TailPos = new dvec3(-0.26, -0.10, -0.08),
                });
                motion.Frames.Add(frame);

                #endregion

                #region Frame 4

                frame = new Frame() { Time = 4 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Chest",
                    HeadPos = new dvec3(-0.0106, 2.6006, -0.2101),
                    TailPos = new dvec3(-0.0182, 2.6565, 0.0342),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Shoulder",
                    ParentName = "Chest",
                    HeadPos = new dvec3(-0.0182, 2.6565, 0.0342),
                    TailPos = new dvec3(-0.1992, 2.6355, -0.0104),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Arm",
                    ParentName = "Shoulder",
                    HeadPos = new dvec3(-0.1992, 2.6355, -0.0104),
                    TailPos = new dvec3(-0.2620, 2.5038, -0.2904),
                });
                motion.Frames.Add(frame);

                #endregion

                #region Frame 5

                frame = new Frame() { Time = 5 };
                frame.Bones.Add(new Bone()
                {
                    Name = "Chest",
                    HeadPos = new dvec3(0, 0, 0),
                    TailPos = new dvec3(-0.12499999999999999, 0, 0.21650635094610968),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Shoulder",
                    ParentName = "Chest",
                    HeadPos = new dvec3(-0.12499999999999999, 0, 0.21650635094610968),
                    TailPos = new dvec3(-0.2988666487320323, 0, 0.16991892282765594),
                });
                frame.Bones.Add(new Bone()
                {
                    Name = "Arm",
                    ParentName = "Shoulder",
                    HeadPos = new dvec3(-0.2988666487320323, 0, 0.16991892282765594),
                    TailPos = new dvec3(-0.2988666487320323, 0, -0.14008107717234405),
                });
                motion.Frames.Add(frame);

                #endregion

                return motion;
            }
        }
    }
}

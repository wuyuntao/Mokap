using HelixToolkit.Wpf;
using Microsoft.Kinect;
using Mokap.Data;
using Mokap.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;

namespace Mokap.Controls
{
    sealed class BodyViewport
    {
        private HelixViewport3D viewport;

        private List<Body> bodies = new List<Body>();

        public BodyViewport(HelixViewport3D viewport)
        {
            this.viewport = viewport;

            bodies.Add(new Body(viewport, 1));
        }

        class Body
        {
            private HelixViewport3D viewport;

            private ulong trackingId;

#if !NO_KINECT
            private Dictionary<JointType, ModelVisual3D> joints = new Dictionary<JointType, ModelVisual3D>();
#endif

            public Body(HelixViewport3D viewport, ulong trackingId)
            {
                this.viewport = viewport;
                this.trackingId = trackingId;

#if !NO_KINECT
                CreateJoints();
#endif
                CreateBones();
            }

#if !NO_KINECT
            private void CreateJoints()
            {
                var reader = new ObjReader(viewport.Dispatcher);
                var model = reader.Read(Path.Combine(Environment.CurrentDirectory, @"Resources\Joint.obj"));

                var jointTypes = Enum.GetValues(typeof(JointType));
                foreach (JointType type in jointTypes)
                {
                    var joint = new ModelVisual3D();
                    joint.Content = model;

                    viewport.Children.Add(joint);

                    joints.Add(type, joint);
                }
            }
#endif

            private void CreateBones()
            {
                var reader = new ObjReader(viewport.Dispatcher);
                var model = reader.Read(Path.Combine(Environment.CurrentDirectory, @"Resources\Bone.obj"));

                var boneTypes = Enum.GetValues(typeof(BoneType));
                foreach (BoneType type in boneTypes)
                {
                    var bone = new ModelVisual3D();
                    bone.Content = model;

                    viewport.Children.Add(bone);

                    // TODO How to index bones?
                }
            }
        }
    }
}
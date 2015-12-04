using HelixToolkit.Wpf;
using Microsoft.Kinect;
using Mokap.Data;
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
        }

        class Body
        {
            private HelixViewport3D viewport;

            private ulong trackingId;

            private Dictionary<JointType, ModelVisual3D> joints = new Dictionary<JointType, ModelVisual3D>();

            public Body(HelixViewport3D viewport, ulong trackingId)
            {
                this.viewport = viewport;
                this.trackingId = trackingId;

                CreateJoints();
                CreateBones();
            }

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
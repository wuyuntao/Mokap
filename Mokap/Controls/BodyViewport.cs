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

        public void Update(BodyFrameData frame)
        {
            foreach (var data in frame.Bodies)
            {
                var body = bodies.Find(b => b.TrackingId == data.TrackingId);
                if (body == null)
                {
                    body = new Body(viewport, data.TrackingId);
                    bodies.Add(body);
                }

                body.Update(data);
            }
        }

        class Body
        {
            private HelixViewport3D viewport;

            private ulong trackingId;

            private Dictionary<JointType, Joint> joints = new Dictionary<JointType, Joint>();

            private Dictionary<BoneType, Bone> bones = new Dictionary<BoneType, Bone>();

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

                    joints.Add(type, new Joint() { Model = joint });
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

                    bones.Add(type, new Bone() { Model = bone });
                }
            }

            public void Update(BodyFrameData.Body data)
            {
                UpdateJoints(data);
                UpdateBones(data);
            }

            private void UpdateJoints(BodyFrameData.Body data)
            {
                foreach (var joint in joints)
                {
                    var position = data.Joints[joint.Key].Position3D;
                    var offset = position - joint.Value.LastPosition;

                    joint.Value.Model.Transform = new TranslateTransform3D(offset);
                    joint.Value.LastPosition = position;
                }
            }

            private void UpdateBones(BodyFrameData.Body data)
            {
                foreach (var bone in bones)
                {
                    var fromPosition = joints[bone.Key.FromJoint()].LastPosition;
                    var toPosition = joints[bone.Key.ToJoint()].LastPosition;

                    var transforms = new Transform3DGroup();

                    var offset = fromPosition - bone.Value.LastPosition;
                    transforms.Children.Add(new TranslateTransform3D(offset));

                    // TODO Add rotation transform. Some similar to LookRotation

                    bone.Value.Model.Transform = transforms;
                    bone.Value.LastPosition = fromPosition;
                }
            }

            public ulong TrackingId
            {
                get { return trackingId; }
            }
        }

        class Joint
        {
            public ModelVisual3D Model { get; set; }

            public Vector3D LastPosition { get; set; }
        }

        class Bone
        {
            public ModelVisual3D Model { get; set; }

            public Vector3D LastPosition { get; set; }
        }
    }
}
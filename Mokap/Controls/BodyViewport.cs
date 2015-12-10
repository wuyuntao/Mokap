using HelixToolkit.Wpf;
using Mokap.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;
using JointType = Mokap.Schemas.RecorderMessages.JointType;

namespace Mokap.Controls
{
    sealed class BodyViewport : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private HelixViewport3D viewport;

        private List<Body> bodies = new List<Body>();

        public BodyViewport(HelixViewport3D viewport)
        {
            this.viewport = viewport;
        }

        protected override void DisposeManaged()
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                var body = bodies[i];

                SafeDispose(ref body);
            }

            base.DisposeManaged();
        }

        public void Update(BodyFrameData frame)
        {
            foreach (var data in frame.Bodies)
            {
                if (data.IsTracked)
                {
                    var body = bodies.Find(b => b.TrackingId == data.TrackingId);
                    if (body == null)
                    {
                        body = new Body(viewport, data);
                        bodies.Add(body);
                    }

                    body.Update(data);
                }
            }
        }

        class Body : Disposable
        {
            private HelixViewport3D viewport;

            private ulong trackingId;

            private Dictionary<JointType, Joint> joints = new Dictionary<JointType, Joint>();

            private Dictionary<BoneType, Bone> bones = new Dictionary<BoneType, Bone>();

            public Body(HelixViewport3D viewport, BodyFrameData.Body data)
            {
                this.viewport = viewport;
                trackingId = data.TrackingId;

                CreateJoints();
                CreateBones(data);
            }

            protected override void DisposeManaged()
            {
                foreach (var joint in joints.Values)
                {
                    viewport.Children.Remove(joint.Model);
                }

                foreach (var bone in bones.Values)
                {
                    viewport.Children.Remove(bone.Model);
                }

                base.DisposeManaged();
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

            private void CreateBones(BodyFrameData.Body data)
            {
                var reader = new ObjReader(viewport.Dispatcher);
                var model = reader.Read(Path.Combine(Environment.CurrentDirectory, @"Resources\Bone.obj"));

                foreach (var boneDef in BoneDef.Bones)
                {
                    var bone = new ModelVisual3D();
                    bone.Content = model;

                    var headPosition = data.Joints[boneDef.HeadJointType].Position3D;
                    var tailPosition = data.Joints[boneDef.TailJointType].Position3D;
                    var length = (headPosition - tailPosition).Length;

                    if (boneDef.MirrorType != boneDef.Type)
                    {
                        var mirrorDef = BoneDef.Find(boneDef.MirrorType);
                        headPosition = data.Joints[mirrorDef.HeadJointType].Position3D;
                        tailPosition = data.Joints[mirrorDef.TailJointType].Position3D;
                        var mirrorLength = (headPosition - tailPosition).Length;

                        length = (length + mirrorLength) / 2;
                    }

                    viewport.Children.Add(bone);

                    bones.Add(boneDef.Type, new Bone() { Model = bone, Length = length });
                }
            }

            public void Update(BodyFrameData.Body data)
            {
                foreach (var boneDef in BoneDef.BonesByHierarchy)
                {
                    var bone = bones[boneDef.Type];

                    if (boneDef.ParentType == BoneType.Root)
                    {
                        bone.HeadPosition = data.Joints[boneDef.HeadJointType].Position3D;

                        UpdateJointPosition(boneDef.HeadJointType, bone.HeadPosition);
                    }
                    else
                    {
                        bone.HeadPosition = bones[boneDef.ParentType].TailPosition;
                    }

                    var rawHeadPosition = data.Joints[boneDef.HeadJointType].Position3D;
                    var rawTailPosition = data.Joints[boneDef.TailJointType].Position3D;
                    var direction = rawTailPosition - rawHeadPosition;
                    direction.Normalize();

                    bone.TailPosition = bone.HeadPosition + direction * bone.Length;

                    UpdateJointPosition(boneDef.TailJointType, bone.TailPosition);

                    Quaternion quaternion;
                    if (boneDef.IsEnd)
                    {
                        var upward = new Vector3D(0, 1, 0);         // TODO upward could be changed according to bones
                        quaternion = KinectHelper.LookRotation(rawTailPosition - rawHeadPosition, upward);
                    }
                    else
                    {
                        quaternion = data.Joints[boneDef.TailJointType].Rotation;
                    }
                    var rotation = new AxisAngleRotation3D(quaternion.Axis, quaternion.Angle);

                    var transforms = new Transform3DGroup();
                    transforms.Children.Add(new ScaleTransform3D(new Vector3D(bone.Length, bone.Length, bone.Length)));
                    transforms.Children.Add(new TranslateTransform3D(bone.HeadPosition));
                    transforms.Children.Add(new RotateTransform3D(rotation, bone.HeadPosition.X, bone.HeadPosition.Y, bone.HeadPosition.Z));

                    bone.Model.Transform = transforms;
                }
            }

            private void UpdateJointPosition(JointType type, Vector3D position)
            {
                var joint = joints[type];
                joint.Model.Transform = new TranslateTransform3D(position);
                joint.HeadPosition = position;
            }

            public ulong TrackingId
            {
                get { return trackingId; }
            }
        }

        class Joint
        {
            public ModelVisual3D Model;

            public Vector3D HeadPosition;
        }

        class Bone
        {
            public ModelVisual3D Model;

            public double Length;

            public Vector3D HeadPosition;

            public Vector3D TailPosition;
        }
    }
}
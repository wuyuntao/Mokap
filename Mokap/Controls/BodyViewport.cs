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
                        body = new Body(viewport, MotionBodyData.CreateBodyData(data));
                        bodies.Add(body);
                    }

                    body.Update(data);
                }
            }
        }

        class Body : Disposable
        {
            private HelixViewport3D viewport;

            private MotionBodyData.Body bodyData;

            private Dictionary<JointType, Visual3D> joints = new Dictionary<JointType, Visual3D>();

            private Dictionary<BoneType, Visual3D> bones = new Dictionary<BoneType, Visual3D>();

            public Body(HelixViewport3D viewport, MotionBodyData.Body bodyData)
            {
                this.viewport = viewport;
                this.bodyData = bodyData;

                CreateJoints();
                CreateBones(bodyData);
            }

            protected override void DisposeManaged()
            {
                foreach (var joint in joints.Values)
                {
                    viewport.Children.Remove(joint);
                }

                foreach (var bone in bones.Values)
                {
                    viewport.Children.Remove(bone);
                }

                base.DisposeManaged();
            }

            private void CreateJoints()
            {
                var jointTypes = Enum.GetValues(typeof(JointType));
                foreach (JointType type in jointTypes)
                {
                    var model = new ModelVisual3D() { Content = JointModel };

                    viewport.Children.Add(model);
                    joints.Add(type, model);
                }
            }

            private void CreateBones(MotionBodyData.Body data)
            {
                foreach (var bone in data.Bones)
                {
                    var model = new ModelVisual3D() { Content = BoneModel };

                    viewport.Children.Add(model);
                    bones.Add(bone.Type, model);
                }
            }

            public void Update(BodyFrameData.Body data)
            {
                var motion = MotionFrameData.CreateBodyData(data, this.bodyData);

                foreach (var bone in motion.Bones)
                {
                    var boneDef = BoneDef.Find(bone.Type);
                    var boneLength = this.bodyData.FindBone(bone.Type).Length;

                    if (boneDef.ParentType == BoneType.Root)
                    {
                        joints[boneDef.HeadJointType].Transform = new TranslateTransform3D(bone.HeadPosition);
                    }

                    joints[boneDef.TailJointType].Transform = new TranslateTransform3D(bone.TailPosition);

                    var rotation = new AxisAngleRotation3D(bone.Rotation.Axis, bone.Rotation.Angle);

                    var transforms = new Transform3DGroup();
                    transforms.Children.Add(new ScaleTransform3D(new Vector3D(boneLength, boneLength, boneLength)));
                    transforms.Children.Add(new TranslateTransform3D(bone.HeadPosition));
                    transforms.Children.Add(new RotateTransform3D(rotation, bone.HeadPosition.X, bone.HeadPosition.Y, bone.HeadPosition.Z));

                    bones[bone.Type].Transform = transforms;
                }
            }

            private Model3DGroup JointModel
            {
                get
                {
                    var reader = new ObjReader(viewport.Dispatcher);
                    return reader.Read(Path.Combine(Environment.CurrentDirectory, @"Resources\Joint.obj"));
                }
            }

            private Model3DGroup BoneModel
            {
                get
                {
                    var reader = new ObjReader(viewport.Dispatcher);
                    return reader.Read(Path.Combine(Environment.CurrentDirectory, @"Resources\Bone.obj"));
                }
            }

            public ulong TrackingId
            {
                get { return bodyData.TrackingId; }
            }
        }
    }
}
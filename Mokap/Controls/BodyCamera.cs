using Mokap.Data;
using Mokap.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FrameEdges = Mokap.Schemas.RecorderMessages.FrameEdges;
using HandState = Mokap.Schemas.RecorderMessages.HandState;
using JointType = Mokap.Schemas.RecorderMessages.JointType;
using TrackingState = Mokap.Schemas.RecorderMessages.TrackingState;

namespace Mokap.Controls
{
    sealed class BodyCamera
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int width;

        private int height;

        private DrawingGroup drawingGroup;

        private DrawingImage drawingImage;

        private readonly Brush handClosedBrush = CreateSolidColorBrush(Settings.Default.HandClosedClor);

        private readonly Brush handOpenBrush = CreateSolidColorBrush(Settings.Default.HandOpenColor);

        private readonly Brush handLassoBrush = CreateSolidColorBrush(Settings.Default.HandLassoColor);

        private readonly Brush trackedJointBrush = CreateSolidColorBrush(Settings.Default.TrackedJointColor);

        private readonly Brush inferredJointBrush = CreateSolidColorBrush(Settings.Default.InferredJointColor);

        private readonly Pen inferredBonePen = CreateSolidColorPen(Settings.Default.InferredBoneColor, 1);

        private readonly Pen[] bodyPens = CreateBodyPens();

        #region Bone

        class Bone
        {
            public JointType From { get; private set; }
            public JointType To { get; private set; }

            public Bone(JointType from, JointType to)
            {
                From = from;
                To = to;
            }
        }

        #endregion

        #region Initialization

        public BodyCamera(Image image, int width, int height)
        {
            this.width = width;
            this.height = height;

            drawingGroup = new DrawingGroup();
            drawingImage = new DrawingImage(drawingGroup);

            image.Source = drawingImage;
        }

        private static Pen[] CreateBodyPens()
        {
            var settings = Settings.Default;

            return new Pen[]
            {
                CreateSolidColorPen( settings.Body1Color, settings.BodyPenThickness),
                CreateSolidColorPen( settings.Body2Color, settings.BodyPenThickness),
                CreateSolidColorPen( settings.Body3Color, settings.BodyPenThickness),
                CreateSolidColorPen( settings.Body4Color, settings.BodyPenThickness),
                CreateSolidColorPen( settings.Body5Color, settings.BodyPenThickness),
                CreateSolidColorPen( settings.Body6Color, settings.BodyPenThickness),
            };
        }

        private static Pen CreateSolidColorPen(System.Drawing.Color color, double thickness)
        {
            return new Pen(CreateSolidColorBrush(color), thickness);
        }

        private static SolidColorBrush CreateSolidColorBrush(System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        #endregion

        public void Update(BodyFrameData frame)
        {
            // TODO what if multiple body is tracked
            var trackedBodyIndex = Array.FindIndex(frame.Bodies, b => b.IsTracked);

            if (trackedBodyIndex >= 0)
            {
                DrawBodies(frame.Bodies);
            }
        }

        #region Draw

        private void DrawBodies(BodyFrameData.Body[] bodies)
        {
            using (var context = drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                context.DrawRectangle(Brushes.Black, null, new Rect(0, 0, width, height));

                int penIndex = 0;
                foreach (var body in bodies)
                {
                    var pen = bodyPens[penIndex++];

                    if (body.IsTracked)
                    {
                        DrawClippedEdges(context, body);

                        DrawBody(context, pen, body.Joints);

                        DrawHand(context, body.HandLeft.State, body.Joints[JointType.HandLeft]);
                        DrawHand(context, body.HandRight.State, body.Joints[JointType.HandRight]);
                    }
                }

                // prevent drawing outside of our render area
                drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, width, height));
            }
        }

        private void DrawBody(DrawingContext context, Pen pen, IDictionary<JointType, BodyFrameData.Joint> joints)
        {
            // Draw the bones
            foreach (var bone in BoneDef.Bones)
            {
                DrawBone(context, pen, joints, bone.HeadJointType, bone.TailJointType);
            }

            // Draw the joints
            foreach (var jointType in joints.Keys)
            {
                Brush brush = null;

                var trackingState = joints[jointType].State;

                if (trackingState == TrackingState.Tracked)
                {
                    brush = trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    brush = inferredJointBrush;
                }

                if (brush != null)
                {
                    context.DrawEllipse(brush, null,
                            joints[jointType].Position2D,
                            Settings.Default.JointThickness,
                            Settings.Default.JointThickness);
                }
            }
        }

        private void DrawBone(DrawingContext context, Pen pen, IDictionary<JointType, BodyFrameData.Joint> joints, JointType jointType1, JointType jointType2)
        {
            var joint1 = joints[jointType1];
            var joint2 = joints[jointType2];

            // If we can't find either of these joints, exit
            if (joint1.State == TrackingState.NotTracked ||
                joint2.State == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            if ((joint1.State != TrackingState.Tracked) || (joint2.State != TrackingState.Tracked))
            {
                pen = inferredBonePen;
            }

            context.DrawLine(pen, joints[jointType1].Position2D, joints[jointType2].Position2D);
        }

        void DrawHand(DrawingContext context, HandState state, BodyFrameData.Joint joint)
        {
            Brush brush;
            switch (state)
            {
                case HandState.Closed:
                    brush = handClosedBrush;
                    break;

                case HandState.Open:
                    brush = handOpenBrush;
                    break;

                case HandState.Lasso:
                    brush = handLassoBrush;
                    break;

                default:
                    brush = null;
                    break;
            }

            if (brush != null)
            {
                context.DrawEllipse(brush, null,
                        joint.Position2D,
                        Settings.Default.HandSize,
                        Settings.Default.HandSize);
            }
        }

        private void DrawClippedEdges(DrawingContext context, BodyFrameData.Body body)
        {
            var thickness = Settings.Default.ClipBoundsThickness;
            var clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, height - thickness, width, thickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, width, thickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, thickness, height));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(width - thickness, 0, thickness, height));
            }
        }

        #endregion
    }
}

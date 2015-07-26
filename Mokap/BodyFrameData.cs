using Microsoft.Kinect;
using Mokap.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using BvhMotion = Mokap.Bvh.Motion;

namespace Mokap
{
    sealed class BodyFrameData
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper;

        /// <summary>
        /// Image width of body frame
        /// </summary>
        private int width;

        /// <summary>
        /// Image height of body frame
        /// </summary>
        private int height;

        /// <summary>
        /// Intermediate storage for the body data received from the camera in 32bit body
        /// </summary>
        private Body[] bodies;

        /// <summary>
        /// definition of bones
        /// </summary>
        private Bone[] bones = CreateBones();

        /// <summary>
        /// Bvh motion
        /// </summary>
        private BvhMotion motion = new BvhMotion();

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image for display
        /// </summary>
        private DrawingImage drawingImage;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = CreateSolidColorBrush(Settings.Default.HandClosedClor);

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = CreateSolidColorBrush(Settings.Default.HandOpenColor);

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = CreateSolidColorBrush(Settings.Default.HandLassoColor);

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = CreateSolidColorBrush(Settings.Default.TrackedJointColor);

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = CreateSolidColorBrush(Settings.Default.InferredJointColor);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = CreateSolidColorPen(Settings.Default.InferredBoneColor, 1);

        /// <summary>
        /// List of colors for each body tracked
        /// </summary
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

        public BodyFrameData(CoordinateMapper coordinateMapper, int width, int height)
        {
            this.coordinateMapper = coordinateMapper;
            this.width = width;
            this.height = height;

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.drawingImage = new DrawingImage(this.drawingGroup);
        }

        private static Bone[] CreateBones()
        {
            var bones = new Bone[24];

            // Torso
            bones[0] = new Bone(JointType.Head, JointType.Neck);
            bones[1] = new Bone(JointType.Neck, JointType.SpineShoulder);
            bones[2] = new Bone(JointType.SpineShoulder, JointType.SpineMid);
            bones[3] = new Bone(JointType.SpineMid, JointType.SpineBase);
            bones[4] = new Bone(JointType.SpineShoulder, JointType.ShoulderRight);
            bones[5] = new Bone(JointType.SpineShoulder, JointType.ShoulderLeft);
            bones[6] = new Bone(JointType.SpineBase, JointType.HipRight);
            bones[7] = new Bone(JointType.SpineBase, JointType.HipLeft);

            // Right Arm
            bones[8] = new Bone(JointType.ShoulderRight, JointType.ElbowRight);
            bones[9] = new Bone(JointType.ElbowRight, JointType.WristRight);
            bones[10] = new Bone(JointType.WristRight, JointType.HandRight);
            bones[11] = new Bone(JointType.HandRight, JointType.HandTipRight);
            bones[12] = new Bone(JointType.WristRight, JointType.ThumbRight);

            // Left Arm
            bones[13] = new Bone(JointType.ShoulderLeft, JointType.ElbowLeft);
            bones[14] = new Bone(JointType.ElbowLeft, JointType.WristLeft);
            bones[15] = new Bone(JointType.WristLeft, JointType.HandLeft);
            bones[16] = new Bone(JointType.HandLeft, JointType.HandTipLeft);
            bones[17] = new Bone(JointType.WristLeft, JointType.ThumbLeft);

            // Right Leg
            bones[18] = new Bone(JointType.HipRight, JointType.KneeRight);
            bones[19] = new Bone(JointType.KneeRight, JointType.AnkleRight);
            bones[20] = new Bone(JointType.AnkleRight, JointType.FootRight);

            // Left Leg
            bones[21] = new Bone(JointType.HipLeft, JointType.KneeLeft);
            bones[22] = new Bone(JointType.KneeLeft, JointType.AnkleLeft);
            bones[23] = new Bone(JointType.AnkleLeft, JointType.FootLeft);

            return bones;
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
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        #endregion

        public bool Update(BodyFrameReference bodyFrameReference)
        {
            using (var bodyFrame = bodyFrameReference.AcquireFrame())
            {
                if (bodyFrame == null)
                {
                    logger.Trace("Abort update since BodyFrame is null");
                    return false;
                }

                var stopwatch = Stopwatch.StartNew();

                if (this.bodies == null || this.bodies.Length != bodyFrame.BodyCount)
                {
                    this.bodies = new Body[bodyFrame.BodyCount];
                }

                bodyFrame.GetAndRefreshBodyData(this.bodies);

                logger.Trace("BodyFrame updated. Spent: {0}ms", stopwatch.ElapsedMilliseconds);
            }

            // TODO what if multiple body is tracked
            var trackedBodyIndex = Array.FindIndex(this.bodies, b => b.IsTracked);

            if (trackedBodyIndex >= 0)
            {
                var stopwatch = Stopwatch.StartNew();

                var body = new KinectBodyAdapter(this.bodies[trackedBodyIndex]);

                if (!this.motion.HasSkeleton)
                {
                    this.motion.CreateSkeleton(body);
                }
                else
                {
                    this.motion.AppendFrame(body);
                }

                // Log frame data to mock bvh generation
                /*
                foreach (var joint in this.motion.Skeleton.Joints)
                {
                    var adpater = (IBodyAdapter)body;
                    var position = adpater.GetJointPosition(joint.Type);
                    var rotation = adpater.GetJointRotation(joint.Type);

                    logger.Trace("Frame:{0},Type:{1},Position:{2:f4},{3:f4},{4:f4},Rotation:{5:f4},{6:f4},{7:f4},{8:f4}"
                            , this.motion.FrameCount, joint.Type
                            , position.X, position.Y, position.Z
                            , rotation.X, rotation.Y, rotation.Z, rotation.W);
                }
                */

                DrawBodies();

                logger.Trace("Update motion data and draw body. Spent: {0}ms", stopwatch.ElapsedMilliseconds);
            }

            return true;
        }

        #region Draw

        private void DrawBodies()
        {
            using (var context = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                context.DrawRectangle(Brushes.Black, null, new Rect(0, 0, this.width, this.height));

                int penIndex = 0;
                foreach (var body in this.bodies)
                {
                    var pen = this.bodyPens[penIndex++];

                    if (body.IsTracked)
                    {
                        DrawClippedEdges(context, body);

                        var joints = body.Joints;

                        // convert the joint points to depth (display) space
                        var jointPoints = new Dictionary<JointType, Point>();

                        foreach (var jointType in joints.Keys)
                        {
                            // sometimes the depth(Z) of an inferred joint may show as negative
                            // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                            var position = joints[jointType].Position;
                            if (position.Z < 0)
                            {
                                position.Z = Settings.Default.InferredZPositionClamp;
                            }

                            var depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                            jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                        }

                        DrawBody(context, pen, joints, jointPoints);

                        DrawHand(context, body.HandLeftState, jointPoints[JointType.HandLeft]);
                        DrawHand(context, body.HandRightState, jointPoints[JointType.HandRight]);
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.width, this.height));
            }
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="context">drawing context to draw to</param>
        /// <param name="pen">specifies color to draw a specific body</param>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        private void DrawBody(DrawingContext context, Pen pen, IReadOnlyDictionary<JointType, Joint> joints, Dictionary<JointType, Point> jointPoints)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                DrawBone(context, pen, joints, jointPoints, bone.From, bone.To);
            }

            // Draw the joints
            foreach (var jointType in joints.Keys)
            {
                Brush brush = null;

                var trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    brush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    brush = this.inferredJointBrush;
                }

                if (brush != null)
                {
                    context.DrawEllipse(brush, null, jointPoints[jointType], Settings.Default.JointThickness, Settings.Default.JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="context">drawing context to draw to</param>
        /// <param name="pen">specifies color to draw a specific bone</param
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType1">first joint of bone to draw</param>
        /// <param name="jointType2">second joint of bone to draw</param>
        private void DrawBone(DrawingContext context, Pen pen, IReadOnlyDictionary<JointType, Joint> joints, Dictionary<JointType, Point> jointPoints, JointType jointType1, JointType jointType2)
        {
            var joint1 = joints[jointType1];
            var joint2 = joints[jointType2];

            // If we can't find either of these joints, exit
            if (joint1.TrackingState == TrackingState.NotTracked ||
                joint2.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            if ((joint1.TrackingState != TrackingState.Tracked) || (joint2.TrackingState != TrackingState.Tracked))
            {
                pen = this.inferredBonePen;
            }

            context.DrawLine(pen, jointPoints[jointType1], jointPoints[jointType2]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="context">drawing context to draw to</param
        /// <param name="state">state of the hand</param>
        /// <param name="position">position of the hand</param>
        void DrawHand(DrawingContext context, HandState state, Point position)
        {
            Brush brush;
            switch (state)
            {
                case HandState.Closed:
                    brush = this.handClosedBrush;
                    break;

                case HandState.Open:
                    brush = this.handOpenBrush;
                    break;

                case HandState.Lasso:
                    brush = this.handLassoBrush;
                    break;

                default:
                    brush = null;
                    break;
            }

            if (brush != null)
            {
                context.DrawEllipse(brush, null, position, Settings.Default.HandSize, Settings.Default.HandSize);
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="context">drawing context to draw to</param>
        /// <param name="body">body to draw clipping information for</param>
        private void DrawClippedEdges(DrawingContext context, Body body)
        {
            var thickness = Settings.Default.ClipBoundsThickness;
            var clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.height - thickness, this.width, thickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.width, thickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, thickness, this.height));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                context.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.width - thickness, 0, thickness, this.height));
            }
        }

        #endregion

        #region Properties

        public Body[] Bodies
        {
            get { return this.bodies; }
        }

        public BvhMotion Motion
        {
            get { return this.motion; }
        }

        public ImageSource Bitmap
        {
            get { return this.drawingImage; }
        }

        #endregion
    }
}

using Microsoft.Kinect;
using Mokap.Properties;
using NLog;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mokap
{
    /// <summary>
    /// Frame Data of depth camera
    /// </summary
    sealed class DepthFrameData
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Image width of depth frame
        /// </summary>
        private int width;

        /// <summary>
        /// Image height of depth frame
        /// </summary>
        private int height;

        /// <summary>
        /// Intermediate storage for the depth data received from the camera in 32bit depth
        /// </summary>
        private ushort[] data;

        /// <summary>
        /// Bitmap to display depth camera
        /// </summary>
        private WriteableBitmap bitmap;

        public DepthFrameData(int width, int height)
        {
            var defaultSystemDPI = Settings.Default.DefaultSystemDPI;

            this.width = width;
            this.height = height;
            this.data = new ushort[width * height];
            this.bitmap = new WriteableBitmap(width, height, defaultSystemDPI, defaultSystemDPI, PixelFormats.Gray8, null);
        }

        public bool Update(DepthFrameReference depthFrameReference)
        {
            using (var depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                {
                    logger.Trace("Abort update since DepthFrame is null");
                    return false;
                }

                var depthFrameDescription = depthFrame.FrameDescription;
                if (this.width == depthFrameDescription.Width && this.height == depthFrameDescription.Height)
                {
                    var stopwatch = Stopwatch.StartNew();

                    depthFrame.CopyFrameDataToArray(this.data);

                    using (var depthBuffer = depthFrame.LockImageBuffer())
                    {
                        this.bitmap.Lock();

                        this.bitmap.WritePixels(
                                new Int32Rect(0, 0, this.bitmap.PixelWidth, this.bitmap.PixelHeight),
                                Array.ConvertAll(this.data, d => MapDepthToByte(d, depthFrame.DepthMinReliableDistance, depthFrame.DepthMaxReliableDistance)),
                                this.bitmap.PixelWidth,
                                0);

                        this.bitmap.Unlock();
                    }

                    logger.Trace("DepthFrame updated. Spent: {0}ms", stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    logger.Error("Size of DepthFrame does not match. Expected: {0}x{1}, Actual: {2}x{3}",
                        this.width, this.height, depthFrameDescription.Width, depthFrameDescription.Height);
                }

                return true;
            }
        }

        private byte MapDepthToByte(ushort depth, ushort minDepth, ushort maxDepth)
        {
            if (depth >= maxDepth)
                return byte.MaxValue;

            if (depth <= minDepth)
                return byte.MinValue;

            return (byte)Math.Round(((float)depth / (maxDepth - minDepth)) * byte.MaxValue);
        }

        #region Properties

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }

        public ushort[] Data
        {
            get { return this.data; }
        }

        public BitmapSource Bitmap
        {
            get { return this.bitmap; }
        }

        #endregion
    }
}
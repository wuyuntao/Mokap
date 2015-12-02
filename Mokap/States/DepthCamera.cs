using Mokap.Data;
using Mokap.Properties;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mokap.States
{
    sealed class DepthCamera
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int width;

        private int height;

        private WriteableBitmap bitmap;

        public DepthCamera(Image image, int width, int height)
        {
            this.width = width;
            this.height = height;

            var defaultSystemDPI = Settings.Default.DefaultSystemDPI;
            bitmap = new WriteableBitmap(width, height, defaultSystemDPI, defaultSystemDPI, PixelFormats.Gray8, null);

            image.Source = bitmap;
        }

        public void Update(DepthFrameData frame)
        {
            if (width != frame.Width || height != frame.Height)
            {
                logger.Error("Size of DepthFrame does not match. Expected: {0}x{1}, Actual: {2}x{3}",
                    width, height, frame.Width, frame.Height);
            }
            else
            {
                var dataBytes = Array.ConvertAll(frame.Data, d => MapDepthToByte(d, frame.MinReliableDistance, frame.MaxReliableDistance));

                bitmap.Lock();

                bitmap.WritePixels(
                        new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                        dataBytes,
                        bitmap.PixelWidth,
                        0);

                bitmap.Unlock();
            }
        }
        
        /*
        public bool Update(Microsoft.Kinect.DepthFrameReference depthFrameReference)
        {
            using (var depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                {
                    logger.Trace("Abort update since DepthFrame is null");
                    return false;
                }

                var depthFrameDescription = depthFrame.FrameDescription;
                if (width == depthFrameDescription.Width && height == depthFrameDescription.Height)
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                    var data = new ushort[width * height];
                    depthFrame.CopyFrameDataToArray(data);

                    using (var depthBuffer = depthFrame.LockImageBuffer())
                    {
                        var dataBytes = Array.ConvertAll(data, d => MapDepthToByte(d, depthFrame.DepthMinReliableDistance, depthFrame.DepthMaxReliableDistance));

                        bitmap.Lock();

                        bitmap.WritePixels(
                                new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                                dataBytes,
                                bitmap.PixelWidth,
                                0);

                        bitmap.Unlock();
                    }

                    logger.Trace("DepthFrame updated. Spent: {0}ms", stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    logger.Error("Size of DepthFrame does not match. Expected: {0}x{1}, Actual: {2}x{3}",
                        width, height, depthFrameDescription.Width, depthFrameDescription.Height);
                }

                return true;
            }
        }
        */

        private byte MapDepthToByte(ushort depth, ushort minDepth, ushort maxDepth)
        {
            if (depth >= maxDepth)
                return byte.MaxValue;

            if (depth <= minDepth)
                return byte.MinValue;

            return (byte)Math.Round(((float)depth / (maxDepth - minDepth)) * byte.MaxValue);
        }
    }
}
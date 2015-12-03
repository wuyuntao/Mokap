using Mokap.Data;
using Mokap.Properties;
using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mokap.Controls
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
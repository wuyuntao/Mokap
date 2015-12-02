using Mokap.Data;
using Mokap.Properties;
using NLog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mokap.States
{
    sealed class ColorCamera
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int width;

        private int height;

        private WriteableBitmap bitmap;

        public ColorCamera(Image image, int width, int height)
        {
            this.width = width;
            this.height = height;

            var defaultSystemDPI = Settings.Default.DefaultSystemDPI;
            bitmap = new WriteableBitmap(width, height, defaultSystemDPI, defaultSystemDPI, PixelFormats.Bgr32, null);

            image.Source = bitmap;
        }

        public void Update(ColorFrameData frame)
        {
            if (width != frame.Width || height != frame.Height)
            {
                logger.Error("Size of DepthFrame does not match. Expected: {0}x{1}, Actual: {2}x{3}",
                    width, height, frame.Width, frame.Height);
            }
            else
            {
                bitmap.Lock();

                bitmap.WritePixels(
                        new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                        frame.Data,
                        bitmap.PixelWidth,
                        0);

                bitmap.Unlock();
            }
        }

        /*
        public bool Update(ColorFrameReference colorFrameReference)
        {
            using (var colorFrame = colorFrameReference.AcquireFrame())
            {
                if (colorFrame == null)
                {
                    logger.Trace("Abort update since ColorFrame is null");
                    return false;
                }

                var colorFrameDescription = colorFrame.FrameDescription;
                if (width == colorFrameDescription.Width && height == colorFrameDescription.Height)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var data = new byte[width * height * sizeof(int)];
                    colorFrame.CopyConvertedFrameDataToArray(data, ColorImageFormat.Rgba);

                    using (var colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        bitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        colorFrame.CopyConvertedFrameDataToIntPtr(
                                bitmap.BackBuffer,
                                (uint)(width * height * sizeof(int)),
                                ColorImageFormat.Bgra);

                        bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

                        bitmap.Unlock();
                    }

                    logger.Trace("ColorFrame updated. Spent: {0}ms", stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    logger.Error("Size of ColorFrame does not match. Expected: {0}x{1}, Actual: {2}x{3}",
                        width, height, colorFrameDescription.Width, colorFrameDescription.Height);
                }

                return true;
            }
        }
        */
    }
}
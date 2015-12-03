using Mokap.Data;
using Mokap.Properties;
using NLog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mokap.Controls
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
                        bitmap.PixelWidth * sizeof(int),
                        0);

                bitmap.Unlock();
            }
        }
    }
}
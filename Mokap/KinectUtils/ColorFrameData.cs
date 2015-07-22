using Microsoft.Kinect;
using Mokap.Properties;
using NLog;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mokap.KinectUtils
{
    /// <summary>
    /// Frame Data of color camera
    /// </summary
    public sealed class ColorFrameData
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Image width of color frame
        /// </summary>
        private int width;

        /// <summary>
        /// Image height of color frame
        /// </summary>
        private int height;

        /// <summary>
        /// Intermediate storage for the color data received from the camera in 32bit color
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Bitmap to display color camera
        /// </summary>
        private WriteableBitmap bitmap;

        public ColorFrameData(int width, int height)
        {
            var defaultSystemDPI = Settings.Default.DefaultSystemDPI;

            this.width = width;
            this.height = height;
            this.data = new byte[width * height * sizeof(int)];
            this.bitmap = new WriteableBitmap(width, height, defaultSystemDPI, defaultSystemDPI, PixelFormats.Bgr32, null);
        }

        public bool Update(MultiSourceFrame multiSourceFrame)
        {
            if (multiSourceFrame == null)
            {
                logger.Trace("Abort update since MultiSourceFrame is null");
                return false;
            }

            using (var colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                if (colorFrame == null)
                {
                    logger.Trace("Abort update since ColorFrame is null");
                    return false;
                }

                var colorFrameDescription = colorFrame.FrameDescription;
                if (this.width == colorFrameDescription.Width && this.height == colorFrameDescription.Height)
                {
                    var stopwatch = Stopwatch.StartNew();

                    colorFrame.CopyConvertedFrameDataToArray(this.data, ColorImageFormat.Rgba);

                    using (var colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.bitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.bitmap.BackBuffer,
                                (uint)(this.width * this.height * sizeof(int)),
                                ColorImageFormat.Bgra);

                        this.bitmap.AddDirtyRect(new Int32Rect(0, 0, this.bitmap.PixelWidth, this.bitmap.PixelHeight));

                        this.bitmap.Unlock();
                    }

                    logger.Trace("ColorFrame updated. Spent: {0}ms", stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    logger.Error("Size of ColorFrame does not match. Expected: {0}x{1}, Actual: {2}x{3}",
                        this.width, this.height, colorFrameDescription.Width, colorFrameDescription.Height);
                }

                return true;
            }
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

        public byte[] Data
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
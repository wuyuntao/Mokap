using Mokap.Controls;
using Mokap.Data;

namespace Mokap.States
{
    abstract class Capturing : MainWindow.State
    {
        protected BodyCamera BodyCamera { get; private set; }

        protected ColorCamera ColorCamera { get; private set; }

        protected DepthCamera DepthCamera { get; private set; }

        protected Capturing(MainWindow mainWindow, Metadata metadata)
            : base(mainWindow)
        {
            BodyCamera = new BodyCamera(mainWindow.BodyCamera, metadata.DepthFrameWidth, metadata.DepthFrameHeight);
            ColorCamera = new ColorCamera(mainWindow.ColorCamera, metadata.ColorFrameWidth, metadata.ColorFrameHeight);
            DepthCamera = new DepthCamera(mainWindow.DepthCamera, metadata.DepthFrameWidth, metadata.DepthFrameHeight);
        }
    }
}
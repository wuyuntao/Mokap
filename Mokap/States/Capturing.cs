using Mokap.Controls;
using Mokap.Data;

namespace Mokap.States
{
    abstract class Capturing : MainWindow.State
    {
        private BodyCamera bodyCamera;

        private ColorCamera colorCamera;

        private DepthCamera depthCamera;

        protected Capturing(MainWindow mainWindow, Metadata metadata)
            : base(mainWindow)
        {
            bodyCamera = new BodyCamera(mainWindow.BodyCamera, metadata.DepthFrameWidth, metadata.DepthFrameHeight);
            colorCamera = new ColorCamera(mainWindow.ColorCamera, metadata.ColorFrameWidth, metadata.ColorFrameHeight);
            depthCamera = new DepthCamera(mainWindow.DepthCamera, metadata.DepthFrameWidth, metadata.DepthFrameHeight);
        }
    }
}

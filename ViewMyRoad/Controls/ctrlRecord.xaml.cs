using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ViewMyRoad.Controls
{
    public sealed partial class ctrlRecord : UserControl
    {
        public MediaCapture MediaCapture { get; private set; }


        public ctrlRecord()
        {
            this.InitializeComponent();
        }

        public async Task SetupCameraViewFinder()
        {
            MediaCapture = new MediaCapture();
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (devices == null) return;
            if (!devices.Any(a=>a.Name.ToLowerInvariant().Contains("back"))) return;
            var devInfo = devices.LastOrDefault(a => a.Name.ToLowerInvariant().Contains("back"));
            await MediaCapture.InitializeAsync(new MediaCaptureInitializationSettings() {VideoDeviceId = devInfo.Id});
            CaptureElement.Source = MediaCapture;
            CaptureElement.FlowDirection=FlowDirection.LeftToRight;
            await MediaCapture.StartPreviewAsync();
            var displayInfo = DisplayInformation.GetForCurrentView();
            displayInfo.OrientationChanged += displayInfo_OrientationChanged;
            displayInfo_OrientationChanged(displayInfo,null);

        }

        void displayInfo_OrientationChanged(DisplayInformation sender, object args)
        {
            if (MediaCapture == null) return;
            MediaCapture.SetPreviewRotation(VideoRotationLookup(sender.CurrentOrientation,true));
            var rotation = VideoRotationLookup(sender.CurrentOrientation, false);
            MediaCapture.SetRecordRotation(rotation);
        }

        private VideoRotation VideoRotationLookup(DisplayOrientations currentOrientation, bool b)
        {
            switch (currentOrientation)
            {
                    case DisplayOrientations.Landscape:
                    return VideoRotation.None;
                    case DisplayOrientations.Portrait:
                    return b ? VideoRotation.Clockwise270Degrees : VideoRotation.Clockwise90Degrees;
                    case DisplayOrientations.LandscapeFlipped:
                    return VideoRotation.Clockwise180Degrees;
                    case DisplayOrientations.PortraitFlipped:
                    return b ? VideoRotation.Clockwise90Degrees : VideoRotation.Clockwise270Degrees;
                default:
                    return VideoRotation.None;

            }
        }
    }
}

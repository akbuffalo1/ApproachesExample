using Android.Hardware.Camera2;

namespace TigerApp.Droid.UI.DeviceCamera
{
    class CameraCaptureStateListener
    {
        public CameraCaptureStateListener()
        {
        }

        public System.Func<CameraCaptureSession, object> OnConfiguredAction { get; set; }
        public System.Func<CameraCaptureSession, object> OnConfigureFailedAction { get; set; }
    }
}
#region using

using Android.Content;
using Android.Runtime;
using Android.Views;

#endregion

//https://developer.xamarin.com/guides/xamarin-forms/dependency-service/device-orientation/

namespace TigerApp.Droid.Services.Platform
{
    public enum DeviceOrientations
    {
        Undefined,
        Landscape,
        Portrait
    }

    public class DeviceOrientationService : IDeviceOrientationService
    {
        public DeviceOrientations GetOrientation()
        {
            IWindowManager windowManager =
                Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            var rotation = windowManager.DefaultDisplay.Rotation;
            bool isLandscape = rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270;
            return isLandscape ? DeviceOrientations.Landscape : DeviceOrientations.Portrait;
        }
    }

    //TODO move to shared and implement for IOS
    public interface IDeviceOrientationService
    {
        DeviceOrientations GetOrientation();
    }
}
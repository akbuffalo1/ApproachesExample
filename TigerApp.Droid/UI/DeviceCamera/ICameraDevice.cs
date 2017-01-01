#region using

using System;

#endregion

namespace TigerApp.Droid.UI.DeviceCamera
{
    public interface ICameraDevice
    {
        bool IsTakingPicture { get;}
        event Action<byte[]> OnPictureTakenEvent;
        void StartCamera();
        void StopCamera();
        void SwitchCamera();
        void TakePicture();
        void AutoFocus();
    }
}
using System;

namespace TigerApp.Droid.UI.DeviceCamera
{
    public interface ICameraPreview
    {
        void SetAspectRatio(int width, int height);
        void SetCameraPreviewCallback(ICameraPreviewViewCallback callback);
        void SetClickDelegate(Action onClickDelegate);
        void Destroy();
    }
}
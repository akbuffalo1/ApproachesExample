namespace TigerApp.Droid.UI.DeviceCamera
{
    public interface ICameraPreviewViewCallback
    {
        void PreviewSurfaceChanged(int width, int height);


        void PreviewSurfaceCreated();


        void PreviewSurfaceDestroyed();
    }
}
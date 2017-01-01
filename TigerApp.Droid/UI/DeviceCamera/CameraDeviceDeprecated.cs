#region using

using System;
using System.Collections.Generic;
using AD;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using TigerApp.Droid.Services.Platform;
using Camera = Android.Hardware.Camera;

#endregion

namespace TigerApp.Droid.UI.DeviceCamera
{
    //NOTE this class uses old deprecated API (deprecated simce API=21)
    //TODO investigate and test // Google's camera orientation vs device orientation is all over the place, it changes per api version, per device type (phone/tablet) and per device brand
    // Credits: http://stackoverflow.com/questions/4645960/how-to-set-android-camera-orientation-properly
    public class CameraDeviceDeprecated : Java.Lang.Object,
        ICameraPreviewViewCallback,
        ICameraDevice,
        Camera.IPictureCallback,
        Camera.IShutterCallback,
        Camera.IAutoFocusCallback
    {
        public bool IsTakingPicture
        {
            get { return _isTakingPicture; }
        }
        public event Action<byte[]> OnPictureTakenEvent;

        const string Tag = nameof(CameraDeviceDeprecated);
        private readonly Context _context;
        private readonly bool _keepRatio;

        private ILogger _logger;
        private Camera _camera;
        private int _cameraIndex;
        private Camera.IPreviewCallback _cameraPreviewCallback;
        private bool _cameraPreviewCallbackWithBuffer;
        public Camera.Size _optimalSize;
        private bool _cameraCreated;
        private bool _surfaceCreated;
        private bool _cameraStarted;
        private bool _isTakingPicture;
        private ICameraPreview _cameraPreview;
        private bool _continueCameraPreviewAfterTakingPicture = false;

        public CameraDeviceDeprecated(Context context, ICameraPreview surfaceView, bool keepRatio)
        {
            _logger = Resolver.Resolve<ILogger>();

            _context = context;
            _keepRatio = keepRatio;
            _cameraPreview = surfaceView;
            _cameraPreview.SetCameraPreviewCallback(this);
            surfaceView.SetClickDelegate(() => {
                try
                {
                    AutoFocus();
                }
                catch (Exception ex) {
                    Console.WriteLine(string.Format("[{0}] : {1}",Tag,ex.Message));
                }
            });
        }

        public void SetPreviewSurface(ICameraPreview surfaceView)
        {
            _cameraPreview = surfaceView;
            _cameraPreview.SetCameraPreviewCallback(this);
        }

        public Camera Camera
        {
            get { return _camera; }
        }

        public void SwitchCamera()
        {
            throw new NotImplementedException();
            //TODO implement and test Camera switch
            int numberOfCameras = Camera.NumberOfCameras;
            if (numberOfCameras > 1)
            {
                _cameraIndex++;
                _cameraIndex %= numberOfCameras;
                _cameraCreated = CreateCamera(_cameraIndex);
                if (_cameraCreated)
                {
                    /*SurfaceChanged(
                        _surfaceHolder,
                        Format.Rgb888, //doesn't matter, omitted by the Surface changed function.
                        Width,
                        Height);*/
                }
                _cameraIndex--;
                if (_cameraIndex < 0)
                    _cameraIndex = numberOfCameras - 1;
            }
        }

        private bool CreateCamera(int cameraIndex)
        {
            try
            {
                if (_cameraStarted && _cameraCreated)
                    StopCamera();
                _camera = Camera.Open(cameraIndex);

                if (_cameraPreview is SurfaceView)
                    _camera.SetPreviewDisplay(((SurfaceView)_cameraPreview).Holder);
                else
                    _camera.SetPreviewTexture(((TextureView)_cameraPreview).SurfaceTexture);
                //BUG never call Lock()-Unlock() it is not works (tested API 22, CyanogenMod 12, Galaxy S2)
                //_camera.Lock();

                //print supported sizes
                //Camera.Parameters parameters = _camera.GetParameters();
                /*IList<Camera.Size> sizes = parameters.SupportedPreviewSizes;

                _logger.Debug(Tag, "supported PREVIEW camera sizes");
                foreach (var size in sizes)
                {
                    _logger.Debug(Tag, "w: {0} h: {1} ratio: {2}", size.Width, size.Height,
                        (float) size.Width/(float) size.Height);
                }

                sizes = parameters.SupportedPictureSizes;

                _logger.Info(Tag, "supported PICTURE camera sizes");
                foreach (var size in sizes)
                {
                    _logger.Info(Tag, "w: {0} h: {1} ratio: {2}", size.Width, size.Height,
                        (float) size.Width/(float) size.Height);
                }*/
            }
            catch (Exception excpt)
            {
                if (_camera != null)
                {
                    _camera.Release();
                    //BUG never call Lock()-Unlock() it is not works (tested on API 22, CyanogenMod 12, Galaxy S2)
                    //camera.Unlock();
                }

                _camera = null;

                while (excpt.InnerException != null)
                    excpt = excpt.InnerException;

                var builder = new AlertDialog.Builder(_context);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle("Unable to create camera");
                alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                alertDialog.SetMessage(excpt.Message);
                alertDialog.SetButton("OK", (s, ev) =>
                {
                    //DO Something
                });
                alertDialog.Show();

                _logger.Error(Tag, "Unable to create camera: {0}", excpt.Message);

                return false;
            }

            return true;
        }

        public void StartCamera()
        {
            _cameraStarted = true;

            if (!_surfaceCreated)
            {
                _logger.Debug(Tag, "surface not created, waiting for creation");
                return;
            }
            int numberOfCameras = Camera.NumberOfCameras;
            if (numberOfCameras > 0 && !_cameraCreated || _camera == null)
            {
                _cameraCreated = CreateCamera(_cameraIndex);
            }

            if(_cameraCreated && _camera!=null)
                _camera.StartPreview();
        }

        //TODO Picture format, size, focus
        public void TakePicture()
        {
            if (_camera != null && !_isTakingPicture)
            {
                _isTakingPicture = true;
                Camera.Parameters p = _camera.GetParameters();
                p.PictureFormat = ImageFormatType.Jpeg;
                p.FocusMode = Camera.Parameters.FocusModeContinuousPicture;                //p.PictureFormat = Android.Graphics.ImageFormatType.Rgb565;
                _camera.SetParameters(p);
                _camera.TakePicture(this, null, this);
            }
        }

        public void AutoFocus() {
            _camera.AutoFocus(this);
        }

        public void StopCamera()
        {
            _cameraStarted = false;

            if (_camera != null)
            {
                _camera.StopPreview();
                if (_cameraPreviewCallbackWithBuffer)
                    _camera.SetPreviewCallbackWithBuffer(null);
                else
                    _camera.SetPreviewCallback(null);
                _cameraPreview.Destroy();
                _camera.SetPreviewTexture(null);
                _camera.SetPreviewDisplay(null);
                //BUG never call Lock()-Unlock() it is not works (tested on API 22, CyanogenMod 12, Galaxy S2)
                //_camera.Unlock();
                _camera.Release();
                _camera = null;
                _cameraPreview = null;
                _optimalSize = null;
                _cameraCreated = false;
            }
        }

        public void PreviewSurfaceCreated()
        {
            _surfaceCreated = true;

            //            if (_cameraStarted)
            //                StartCamera();
        }

        public void PreviewSurfaceDestroyed()
        {
            // Surface will be destroyed when we return, so stop the preview.
            // Because the CameraDevice object is not a shared resource, it's very
            // important to release it when the activity is paused.
            _surfaceCreated = false;
            StopCamera();
        }

        private static Camera.Size GetOptimalPreviewSize(IList<Camera.Size> sizes, int w, int h, int maxWidth,
            int maxHeight)
        {
            const double ASPECT_TOLERANCE = 0.05;
            double targetRatio = (double)w / h;

            if (sizes == null)
                return null;

            Camera.Size optimalSize = null;
            double minDiff = Double.MaxValue;

            int targetHeight = h;

            // Try to find an size match aspect ratio and size
            for (int i = 0; i < sizes.Count; i++)
            {
                Camera.Size size = sizes[i];

                if (size.Width > maxWidth || size.Height > maxHeight)
                    continue;

                double ratio = (double)size.Width / size.Height;

                if (Math.Abs(ratio - targetRatio) > ASPECT_TOLERANCE)
                    continue;

                if (Math.Abs(size.Height - targetHeight) < minDiff)
                {
                    optimalSize = size;
                    minDiff = Math.Abs(size.Height - targetHeight);
                }
            }

            // Cannot find the one match the aspect ratio, ignore the requirement
            if (optimalSize == null)
            {
                minDiff = Double.MaxValue;
                for (int i = 0; i < sizes.Count; i++)
                {
                    Camera.Size size = sizes[i];

                    if (Math.Abs(size.Height - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = Math.Abs(size.Height - targetHeight);
                    }
                }
            }

            return optimalSize;
        }

        public void PreviewSurfaceChanged(int w, int h)
        {
            if (!_cameraCreated && _cameraStarted)
            {
                StartCamera();

                if (!_cameraCreated)
                    return;
            }
            else if (!_cameraCreated)
                return;

            // Now that the size is known, set up the camera parameters and begin
            // the preview.
            IList<Camera.Size> sizes;
            Camera.Parameters parameters;
            var orService = Resolver.Resolve<IDeviceOrientationService>();
            var orientation = orService.GetOrientation();
            parameters = _camera.GetParameters();
            sizes = parameters.SupportedPreviewSizes;

            //if low performance choose different max
            //int maxWidth = 720, maxHeight = 800;
            int maxWidth = w, maxHeight = h;
            _optimalSize = GetOptimalPreviewSize(sizes, w, h, maxWidth, maxHeight);

            if (orientation == DeviceOrientations.Portrait)
            {
                _camera.SetDisplayOrientation(90);

                if (_keepRatio)
                {
                    if (_keepRatio)
                        _cameraPreview.SetAspectRatio(_optimalSize.Height, _optimalSize.Width);
                }
            }
            else
            {
                _camera.SetDisplayOrientation(0);

                if (_keepRatio)
                    _cameraPreview.SetAspectRatio(_optimalSize.Width, _optimalSize.Height);
            }

            parameters.SetPreviewSize(_optimalSize.Width, _optimalSize.Height);
            _camera.SetParameters(parameters);


            _logger.Info(Tag, "optimal camera size: {0} x {1}", _optimalSize.Width, _optimalSize.Height);

            if (_cameraPreviewCallback != null)
            {
                if (_cameraPreviewCallbackWithBuffer)
                {
                    int bufferSize = _optimalSize.Width * (_optimalSize.Height >> 1) * 3;
                    _camera.SetPreviewCallbackWithBuffer(_cameraPreviewCallback);
                    for (int i = 0; i < 1; ++i)
                        _camera.AddCallbackBuffer(new byte[bufferSize]);
                }
                else
                    _camera.SetPreviewCallback(_cameraPreviewCallback);
            }

            if (_cameraStarted)
                _camera.StartPreview();

            #region keep ratio layout

            /*
             * TODO layout preview, center etc
            var view = (View) _cameraPreview;
            var par = (View)Parent;
            int width = par.Right - par.Left;
            int height = par.Bottom - par.Top;

            int previewWidth = _optimalSize.Height;
            int previewHeight = _optimalSize.Width;
            int l, t, r, b;

            // Center the child SurfaceView within the parent.

            if (previewHeight > height)
            {
                int scaledChildWidth = previewWidth * height / previewHeight;
                l = (width - scaledChildWidth) / 2;
                t = 0;
                r = (width + scaledChildWidth) / 2;
                b = height;
            }
            else
            {
                int scaledChildHeight = previewHeight * width / previewWidth;
                l = 0;
                t = (height - scaledChildHeight) / 2;
                r = width;
                b = (height + scaledChildHeight) / 2;
            }
            Layout(l, t, r, b);
            */

            #endregion
        }

        public void OnPictureTaken(byte[] data, Camera camera)
        {
            if (data != null)
            {
                _logger.Debug(Tag, "picture taken");
                //TODO rotate or not? :)
                //Bitmap bmp = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            }
            else
            {
                _logger.Error(Tag, "No image is recevide from PictureTakenEventArgs");
            }

            if(_continueCameraPreviewAfterTakingPicture)
                _camera.StartPreview();

            OnPictureTakenEvent?.Invoke(data);

            _isTakingPicture = false;
        }

        public void OnAutoFocus(bool success, Camera camera)
        {
        }

        public void OnShutter()
        {
        }
    }
}
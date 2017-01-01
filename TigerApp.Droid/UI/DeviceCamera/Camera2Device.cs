#region using

using System;
using AD;
using Android.Content;
using Android.Util;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;
using System.Collections.Generic;
using Android.App;
using Android.Media;

#endregion

namespace TigerApp.Droid.UI.DeviceCamera
{
    public class Camera2Device : ICameraPreviewViewCallback, ICameraDevice
    {
        const string Tag = nameof(Camera2Device);
        public const int REQUEST_CAMERA_PERMISSION = 200;
        private readonly Context _context;
        private readonly bool _keepRatio;

        private ILogger _logger;
        private bool _isTakingPicture;
        private ICameraPreview _cameraPreview;
        private Size _imageSize;
        private Camera2DStateCallBack _stateCallback;

        public Camera2Device(Context ctx, ICameraPreview preview, bool keepRatio) 
        {
            _logger = Resolver.Resolve<ILogger>();

            _context = ctx;
            _keepRatio = keepRatio;
            _cameraPreview = preview;
            _cameraPreview.SetCameraPreviewCallback(this);
            _cameraPreview.SetClickDelegate(() =>
            {
                try
                {
                    AutoFocus();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("[{0}] : {1}", Tag, ex.Message));
                }
            });
            /*if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(ctx, Android.Manifest.Permission.Camera) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions((ctx as Android.App.Activity), new String[] { Android.Manifest.Permission.Camera }, REQUEST_CAMERA_PERMISSION);
            }
            else {
                _permGranted = true;
                //_openCamera(ctx);
            }*/
        }

        private async void _openCameraWithPrmission() {
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(_context, Android.Manifest.Permission.Camera) != Permission.Granted)
            {
                var dict = await AD.Resolver.Resolve<AD.Plugins.Permissions.IPermissions>().RequestPermissionsAsync(AD.Plugins.Permissions.Permission.Camera);
                if (dict[AD.Plugins.Permissions.Permission.Camera] == AD.Plugins.Permissions.PermissionStatus.Granted)
                    _openCamera(_context);
            }else{
                _openCamera(_context);
            }
        }

        private void _openCamera(Context ctx) { 
            CameraManager cameraManager = (CameraManager)ctx.GetSystemService(Context.CameraService);
            String cameraId = cameraManager.GetCameraIdList()[0];
            CameraCharacteristics cc = cameraManager.GetCameraCharacteristics(cameraId);
            StreamConfigurationMap streamConfigs = (StreamConfigurationMap)cc.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
            _imageSize = streamConfigs.GetOutputSizes(Java.Lang.Class.FromType(typeof(SurfaceTexture)))[0];
            // Add permission for camera and let user grant the permission

            _stateCallback = new Camera2DStateCallBack(_cameraPreview, _imageSize);
            _stateCallback.OnPictureTaken += (object sender, byte[] bytes) => {
                OnPictureTakenEvent?.Invoke(bytes);
            };
            cameraManager.OpenCamera(cameraId, _stateCallback, null);
        }

        public bool IsTakingPicture { 
            get {
                return _isTakingPicture;
            }
        }
        public event Action<byte[]> OnPictureTakenEvent;

        public void PreviewSurfaceChanged(int width, int height)
        {
            StartCamera();
        }

        public void PreviewSurfaceCreated()
        {
            _openCameraWithPrmission();
        }

        public void PreviewSurfaceDestroyed()
        {
            StopCamera();
        }

        public void StartCamera()
        {
            try
            {
                _stateCallback.StartPreview();
            }
            catch (Exception ex){
                
            }
        }

        public void StopCamera()
        {
            _stateCallback?.CameraDevice?.Close();
        }

        public void SwitchCamera()
        {
        }

        public void AutoFocus()
        {
        }

        public void TakePicture()
        {
            _isTakingPicture = true;
            _stateCallback.TakePicture(_context as Activity);
        }

        private void ConfigureTransform(int viewWidth, int viewHeight)
        {
            Activity activity = _context as Activity;
            if (_cameraPreview == null || _imageSize == null || activity == null)
            {
                return;
            }

            SurfaceOrientation rotation = activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, _imageSize.Width, _imageSize.Height);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = System.Math.Max((float)viewHeight / _imageSize.Height, (float)viewWidth / _imageSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * ((int)rotation - 2), centerX, centerY);
            }
            (_cameraPreview as AutoFitTextureView).SetTransform(matrix);
        }
    }

    public class Camera2DStateCallBack : CameraDevice.StateCallback
    {
        public CameraDevice CameraDevice;
        private CaptureRequest.Builder PreviewBuilder;
        private CameraCaptureSession _previewSession;
        private ICameraPreview _preview;
        private Size _previewSize;
        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();
        public event EventHandler<byte[]> OnPictureTaken;
        private bool SurfaceAvailable { 
            get {
                var isTextureView = (_preview as AutoFitTextureView) != null;
                if (isTextureView)
                    return (_preview as AutoFitTextureView).IsAvailable;
                var isSurfaceView = (_preview as AutoFitSurfaceView) != null;
                if (isSurfaceView)
                    return (_preview as AutoFitSurfaceView).Holder != null && !(_preview as AutoFitSurfaceView).Holder.IsCreating;
                return false;
            }
        }

        private Surface PreviewSurface { 
            get {
                if ((_preview as AutoFitTextureView) != null)
                {
                    var texture = (_preview as AutoFitTextureView).SurfaceTexture;
                    if (texture == null)
                        return null;
                    texture.SetDefaultBufferSize(_previewSize.Width, _previewSize.Height);
                    return new Surface(texture);
                }
                else if ((_preview as AutoFitSurfaceView) != null){
                    var holder = (_preview as AutoFitSurfaceView).Holder;
                    if (holder == null)
                        return null;
                    else
                        return holder.Surface;
                }
                return null;
            }
        }

        public Camera2DStateCallBack(ICameraPreview preview, Size previewSize) 
        {
            _preview = preview;
            _previewSize = previewSize;
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
        }

        public override void OnOpened(CameraDevice camera)
        {
            CameraDevice = camera;
            StartPreview();
        }

        public override void OnClosed(CameraDevice camera)
        {
            base.OnClosed(camera);
            CameraDevice?.Close();
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            CameraDevice.Close();
            CameraDevice = null;
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            CameraDevice.Close();
            CameraDevice = null;
        }

        public void StartPreview()
        {
            if (CameraDevice == null || !SurfaceAvailable || _previewSize == null)
            {
                return;
            }
            try
            {
                Surface surface = PreviewSurface;

                PreviewBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                PreviewBuilder.AddTarget(surface);

                // Here, we create a CameraCaptureSession for camera preview.
                CameraDevice.CreateCaptureSession(new List<Surface>() { surface },
                    new CameraCaptureStateListener()
                    {
                        OnConfiguredAction = (CameraCaptureSession session) =>
                        {
                            _previewSession = session;
                            UpdatePreview();
                        }
                    },
                    null);


            }
            catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "Camera2BasicFragment", ex.StackTrace);
            }
        }


        /// <summary>
        /// Updates the camera preview, StartPreview() needs to be called in advance
        /// </summary>
        private void UpdatePreview()
        {
            if (CameraDevice == null)
            {
                return;
            }

            try
            {
                // The camera preview can be run in a background thread. This is a Handler for the camere preview
                SetUpCaptureRequestBuilder(PreviewBuilder);
                HandlerThread thread = new HandlerThread("CameraPreview");
                thread.Start();
                Handler backgroundHandler = new Handler(thread.Looper);

                // Finally, we start displaying the camera preview
                _previewSession.SetRepeatingRequest(PreviewBuilder.Build(), null, backgroundHandler);
            }
            catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "Camera2BasicFragment", ex.StackTrace);
            }
        }

        private void SetUpCaptureRequestBuilder(CaptureRequest.Builder builder)
        {
            // In this sample, w just let the camera device pick the automatic settings
            builder.Set(CaptureRequest.ControlMode, new Java.Lang.Integer((int)ControlMode.Auto));
        }

        public void TakePicture(Activity activity) { 
            try
            {
                if (activity == null || CameraDevice == null)
                {
                    return;
                }
                CameraManager manager = (CameraManager)activity.GetSystemService(Context.CameraService);

                // Pick the best JPEG size that can be captures with this CameraDevice
                CameraCharacteristics characteristics = manager.GetCameraCharacteristics(CameraDevice.Id);
                Size[] jpegSizes = null;
                if (characteristics != null)
                {
                    jpegSizes = ((StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap)).GetOutputSizes((int)ImageFormatType.Jpeg);
                }
                int width = 640;
                int height = 480;
                if (jpegSizes != null && jpegSizes.Length > 0)
                {
                    width = jpegSizes[0].Width;
                    height = jpegSizes[0].Height;
                }

                // We use an ImageReader to get a JPEG from CameraDevice
                // Here, we create a new ImageReader and prepare its Surface as an output from the camera
                ImageReader reader = ImageReader.NewInstance(width, height, ImageFormatType.Jpeg, 1);
                List<Surface> outputSurfaces = new List<Surface>(2);
                outputSurfaces.Add(reader.Surface);
                outputSurfaces.Add(PreviewSurface);

                CaptureRequest.Builder captureBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                captureBuilder.AddTarget(reader.Surface);
                SetUpCaptureRequestBuilder(captureBuilder);
                // Orientation
                SurfaceOrientation rotation = activity.WindowManager.DefaultDisplay.Rotation;
                captureBuilder.Set(CaptureRequest.JpegOrientation, new Java.Lang.Integer(ORIENTATIONS.Get((int)rotation)));

                ImageAvailableListener readerListener = new ImageAvailableListener() { HandleBytes = HandlePictureBytes };

                HandlerThread thread = new HandlerThread("CameraPicture");
                thread.Start();
                Handler backgroundHandler = new Handler(thread.Looper);
                reader.SetOnImageAvailableListener(readerListener, backgroundHandler);

                CameraCaptureListener captureListener = new CameraCaptureListener();

                CameraDevice.CreateCaptureSession(outputSurfaces, new CameraCaptureStateListener()
                {
                    OnConfiguredAction = (CameraCaptureSession session) =>
                    {
                        try
                        {
                            session.Capture(captureBuilder.Build(), captureListener, backgroundHandler);
                        }
                        catch (CameraAccessException ex)
                        {
                            Log.WriteLine(LogPriority.Info, "Capture Session error: ", ex.ToString());
                        }
                    }
                }, backgroundHandler);
            }
            catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "Taking picture error: ", ex.StackTrace);
            }
        }

        private void HandlePictureBytes(byte[] bytes) {
            OnPictureTaken?.Invoke(this,bytes);
        }

        private class CameraCaptureStateListener : CameraCaptureSession.StateCallback
        {
            public Action<CameraCaptureSession> OnConfigureFailedAction;
            public override void OnConfigureFailed(CameraCaptureSession session)
            {
                if (OnConfigureFailedAction != null)
                {
                    OnConfigureFailedAction(session);
                }
            }

            public Action<CameraCaptureSession> OnConfiguredAction;
            public override void OnConfigured(CameraCaptureSession session)
            {
                if (OnConfiguredAction != null)
                {
                    OnConfiguredAction(session);
                }
            }

        }

        private class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
        {
            public Action<byte[]> HandleBytes;

            public void OnImageAvailable(ImageReader reader)
            {
                Image image = null;
                image = reader.AcquireLatestImage();
                Java.Nio.ByteBuffer buffer = image.GetPlanes()[0].Buffer;
                var bytes = new byte[buffer.Capacity()];
                buffer.Get(bytes);
                HandleBytes?.Invoke(bytes);
            }

        }

        private class CameraCaptureListener : CameraCaptureSession.CaptureCallback
        {
            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
            }
        }

    }
}
#region using

using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

#endregion

namespace TigerApp.Droid.UI.DeviceCamera
{
    public class AutoFitTextureView : TextureView, TextureView.ISurfaceTextureListener, ICameraPreview
    {
        private int _mRatioWidth;
        private int _mRatioHeight;
        private ICameraPreviewViewCallback _callback;

        public AutoFitTextureView(Context context)
            : this(context, null)
        {
            init();
        }

        public AutoFitTextureView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
            init();
        }

        public AutoFitTextureView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            init();
        }

        public AutoFitTextureView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            init();
        }

        private void init()
        {
            SurfaceTextureListener = this;
        }

        public void SetClickDelegate(Action onClickDelegate) {
            this.Click += (sender, e) =>
            {
                if(onClickDelegate != null)
                    onClickDelegate();
            };
        }

        public void SetCameraPreviewCallback(ICameraPreviewViewCallback callback)
        {
            _callback = callback;
        }

        public void Destroy()
        {
            SurfaceTextureListener = null;
            _callback = null;
            Dispose();
        }

        public void SetAspectRatio(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Size cannot be negative.");

            _mRatioWidth = width;
            _mRatioHeight = height;

            RequestLayout();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            if (0 == _mRatioWidth || 0 == _mRatioHeight)
            {
                SetMeasuredDimension(width, height);
            }
            else
            {
                int h = height, w = width;
                if (width < (float) height*_mRatioWidth/_mRatioHeight)
                {
                    h = width*_mRatioHeight/_mRatioWidth;
                    SetMeasuredDimension(width, h);
                }
                else
                {
                    w = height*_mRatioWidth/_mRatioHeight;
                    SetMeasuredDimension(w, height);
                }
            }
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            _callback?.PreviewSurfaceCreated();
            _callback?.PreviewSurfaceChanged(width, height);
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            _callback?.PreviewSurfaceDestroyed();
            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            _callback?.PreviewSurfaceChanged(width, height);
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }
    }
}
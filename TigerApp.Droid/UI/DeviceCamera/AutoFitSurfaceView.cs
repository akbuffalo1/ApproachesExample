#region using

using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

#endregion

namespace TigerApp.Droid.UI.DeviceCamera
{
    public class AutoFitSurfaceView : SurfaceView, ISurfaceHolderCallback, ICameraPreview

    {
        private int _mRatioWidth;
        private int _mRatioHeight;
        private ISurfaceHolder _surfaceHolder;
        private ICameraPreviewViewCallback _callback;

        public AutoFitSurfaceView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AutoFitSurfaceView(Context context) : base(context)
        {
            init();
        }

        public AutoFitSurfaceView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init();
        }

        public AutoFitSurfaceView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            init();
        }

        public AutoFitSurfaceView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            init();
        }

        private void init()
        {
            _surfaceHolder = Holder;
            _surfaceHolder.AddCallback(this);
        }

        public void SetClickDelegate(Action onClickDelegate)
        {
            this.Click += (sender, e) =>
            {
                if (onClickDelegate != null)
                    onClickDelegate();
            };
        }

        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
            CheckHolder();

            _callback?.PreviewSurfaceChanged(width, height);
        }

        public void SetCameraPreviewCallback(ICameraPreviewViewCallback callback)
        {
            _callback = callback;
        }

        public void Destroy()
        {
            _surfaceHolder.RemoveCallback(this);
            _surfaceHolder = null;
            _callback = null;
            Dispose();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            CheckHolder();

            _callback?.PreviewSurfaceCreated();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            _callback?.PreviewSurfaceDestroyed();
        }

        private void CheckHolder()
        {
            if (_surfaceHolder != Holder)
            {
                if (_surfaceHolder != null)
                    _surfaceHolder.RemoveCallback(this);

                _surfaceHolder = Holder;
                _surfaceHolder.AddCallback(this);
            }
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
    }
}
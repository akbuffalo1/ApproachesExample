#region using

using Android.Content;
using Android.Graphics;
using Android.Views;

#endregion

namespace TigerApp.Droid.UI.ToolTips
{
    public class OverlayView : View
    {
        private static readonly int DefaultOverlayCircleOffsetRes =
            Resource.Dimension.simpletooltip_overlay_circle_offset;

        private static readonly int DefaultOverlayAlphaRes = Resource.Integer.simpletooltip_overlay_alpha;
        private readonly bool _drawOval;

        private readonly float _offset;
        private RectF _anchorRect;
        private View _anchorView;
//        private Bitmap _bitmap;
        private int _height;
        private bool _invalidated = true;
//        private Canvas _osCanvas;
        private int _width;

        internal OverlayView(Context context, View anchorView, bool drawOval) : base(context)
        {
            _anchorView = anchorView;
            _drawOval = drawOval;
            _offset = context.Resources.GetDimension(DefaultOverlayCircleOffsetRes);
        }

        public bool InEditMode
        {
            get { return true; }
        }

        public virtual View AnchorView
        {
            get { return _anchorView; }
            set
            {
                _anchorView = value;
                Invalidate();
            }
        }

        /* protected override void DispatchDraw(Canvas canvas)
        {
            if (_invalidated)
            {
                CreateWindowFrame();
            }

            canvas.DrawBitmap(_bitmap, 0, 0, null);
        }*/

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            _width = MeasuredWidth;
            _height = MeasuredHeight;
            DrawFRame(canvas);
        }

        //Bitmap throws out of memory sometimes
        /* private void CreateWindowFrame()
        {
            _width = MeasuredWidth;
            _height = MeasuredHeight;
            if (_width <= 0 || _height <= 0)
                return;

            if (_bitmap != null && !_bitmap.IsRecycled)
                _bitmap.Recycle();

            _bitmap = Bitmap.CreateBitmap(_width, _height, Bitmap.Config.Argb8888); // Bitmap.Config.ARGB_8888

            _osCanvas = new Canvas(_bitmap);

            DrawFRame();
        }*/

        public void DrawFRame(Canvas canvas)
        {
            var outerRectangle = new RectF(0, 0, _width, _height);

            var paint = new Paint(PaintFlags.AntiAlias);
            paint.Color = Color.Black;
            paint.AntiAlias = true;
            paint.Alpha = Resources.GetInteger(DefaultOverlayAlphaRes);
            canvas.DrawRect(outerRectangle, paint);

            paint.Color = Color.Transparent;
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcOut));

            var overlayRecr = SimpleTooltipUtils.CalculeRectInWindow(this);

            _anchorRect = new RectF();
            /* if (_drawOval)
            {
                _anchorRect = SimpleTooltipUtils.CalculeRectInWindow(_anchorView);
                var left = _anchorRect.Left - overlayRecr.Left;
                var top = _anchorRect.Top - overlayRecr.Top;

                var oval = new RectF(
                    left - _offset,
                    top - _offset,
                    left + _anchorView.MeasuredWidth + _offset,
                    top + _anchorView.MeasuredHeight + _offset
                    );

                canvas.DrawOval(oval, paint);
            }*/

            _invalidated = false;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            _invalidated = true;
        }
    }
}
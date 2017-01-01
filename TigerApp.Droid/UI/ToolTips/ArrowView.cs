#region using

using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

#endregion

namespace TigerApp.Droid.UI.ToolTips
{
    internal class ArrowView : View
    {
        public ArrowView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ArrowView(Context context) : base(context)
        {
            ArrowWidth = SimpleTooltipUtils.PxFromDp(60);
        }

        public ArrowView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ArrowView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ArrowView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public float ArrowWidth { get; set; } = 20;

        public int ArrowHeight { get; set; } = 100;

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            var paintA = new Paint(PaintFlags.AntiAlias);
            paintA.Color = Color.White;
            paintA.AntiAlias = true;
            var path = new Path();

            path.MoveTo(ArrowWidth*0.5f, 0);
            path.LineTo(0, ArrowHeight);
            path.LineTo(ArrowWidth, ArrowHeight);
            path.LineTo(ArrowWidth*0.5f, 0);
            path.Close();
            canvas.DrawPath(path, paintA);
        }
    }
}
#region using

using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;

#endregion

namespace TigerApp.Droid.UI
{
    public class TintableButton : Button
    {
        private ColorStateList _tint;

        public TintableButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public TintableButton(Context context) : base(context)
        {
        }

        public TintableButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init(context, attrs, 0);
        }

        public TintableButton(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            init(context, attrs, defStyleAttr);
        }

        public TintableButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            init(context, attrs, defStyleAttr);
        }

        private void init(Context context, IAttributeSet attrs, int defStyle)
        {
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.TintableButton, defStyle, 0);
            _tint = a.GetColorStateList(Resource.Styleable.TintableButton_tint);
            a.Recycle();
        }

        protected override void DrawableStateChanged()
        {
            base.DrawableStateChanged();
            if (_tint != null)
            {
                var state = GetDrawableState();
                var color = new Color(_tint.GetColorForState(state, Color.Black));
                Background.SetColorFilter(color, PorterDuff.Mode.Multiply);
            }
        }
    }
}
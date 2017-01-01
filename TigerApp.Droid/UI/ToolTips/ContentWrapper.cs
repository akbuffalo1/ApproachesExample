#region using

using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

#endregion

namespace TigerApp.Droid.UI.ToolTips
{
    internal class ContentWrapper : FrameLayout
    {
        public ContentWrapper(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ContentWrapper(Context context) : base(context)
        {
        }

        public ContentWrapper(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ContentWrapper(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
        }

        public ContentWrapper(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }
    }
}
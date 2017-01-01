using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TigerApp.Droid.UI.Coupons
{
    public class DynamicLayout : LinearLayout
    {
        public DynamicLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public DynamicLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }

        public DynamicLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public DynamicLayout(Context context) : base(context) { }

        private IViewCreator _adapter;
        public IViewCreator Adapter
        {
            set
            {
                _adapter = value;
                for (var i = 0; i < _adapter.GetCount(); i++)
                {
                    AddView(_adapter.CreateView(i));
                }
                Invalidate();
            }
            get { return _adapter; }
        }
    }

    public interface IViewCreator {
        View CreateView(int position);
        int GetCount();
    }
}


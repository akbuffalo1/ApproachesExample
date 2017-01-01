#region using

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

#endregion

namespace TigerApp.Droid.Utils
{
    public class ScreenUtils
    {
        public static int Dp2Px(Context context, int dip)
        {
            DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dip, displayMetrics);
        }

        public static Point GetWindowSize(Context context)
        {
            IWindowManager wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            Point size = new Point();
            wm.DefaultDisplay.GetSize(size);
            return size;
        }
    }
}
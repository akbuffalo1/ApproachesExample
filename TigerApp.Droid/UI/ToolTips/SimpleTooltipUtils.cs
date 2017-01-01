#region using

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;

#endregion

namespace TigerApp.Droid.UI.ToolTips
{
    public sealed class SimpleTooltipUtils
    {
        private SimpleTooltipUtils()
        {
        }

        public static RectF CalculeRectOnScreen(View view)
        {
            var location = new int[2];
            view.GetLocationOnScreen(location);
            return new RectF(location[0], location[1], location[0] + view.MeasuredWidth,
                location[1] + view.MeasuredHeight);
        }

        public static RectF CalculeRectInWindow(View view)
        {
            var location = new int[2];
            view.GetLocationInWindow(location);
            return new RectF(location[0], location[1], location[0] + view.MeasuredWidth,
                location[1] + view.MeasuredHeight);
        }

        public static float DpFromPx(float px)
        {
            return px/Resources.System.DisplayMetrics.Density;
        }

        public static float PxFromDp(float dp)
        {
            return dp*Resources.System.DisplayMetrics.Density;
        }

        public static void SetWidth(View view, float width)
        {
            var @params = view.LayoutParameters;
            if (@params == null)
            {
                @params = new ViewGroup.LayoutParams((int) width, view.Height);
            }
            else
            {
                @params.Width = (int) width;
            }
            view.LayoutParameters = @params;
        }

        public static void SetX(View view, int x)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                view.SetX(x);
            }
            else
            {
                var marginParams = GetOrCreateMarginLayoutParams(view);
                marginParams.LeftMargin = x - view.Left;
                view.LayoutParameters = marginParams;
            }
        }

        public static void SetY(View view, int y)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                view.SetX(y);
            }
            else
            {
                var marginParams = GetOrCreateMarginLayoutParams(view);
                marginParams.TopMargin = y - view.Top;
                view.LayoutParameters = marginParams;
            }
        }

        private static ViewGroup.MarginLayoutParams GetOrCreateMarginLayoutParams(View view)
        {
            var lp = view.LayoutParameters;
            if (lp != null)
            {
                if (lp is ViewGroup.MarginLayoutParams)
                {
                    return (ViewGroup.MarginLayoutParams) lp;
                }
                return new ViewGroup.MarginLayoutParams(lp);
            }
            return new ViewGroup.MarginLayoutParams(view.Width, view.Height);
        }

        public static void RemoveOnGlobalLayoutListener(View view, ViewTreeObserver.IOnGlobalLayoutListener listener)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                view.ViewTreeObserver.RemoveOnGlobalLayoutListener(listener);
            }
            else
            {
                //noinspection deprecation
                view.ViewTreeObserver.RemoveGlobalOnLayoutListener(listener);
            }
        }

        public static void RemoveOnGlobalLayoutListener(View view)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                view.ViewTreeObserver.RemoveOnGlobalLayoutListener(null);
            }
            else
            {
                //noinspection deprecation
                view.ViewTreeObserver.RemoveGlobalOnLayoutListener(null);
            }
        }

        public static void SetTextAppearance(TextView tv, int textAppearanceRes)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                tv.SetTextAppearance(textAppearanceRes);
            }
            else
            {
                //noinspection deprecation
                tv.SetTextAppearance(tv.Context, textAppearanceRes);
            }
        }

        public static int GetColor(Context context, int colorRes)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                return context.GetColor(colorRes);
            }
            //noinspection deprecation
            return context.Resources.GetColor(colorRes);
        }

        public static Drawable GetDrawable(Context context, int drawableRes)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                return context.GetDrawable(drawableRes);
            }
            //noinspection deprecation
            return context.Resources.GetDrawable(drawableRes);
        }
    }
}
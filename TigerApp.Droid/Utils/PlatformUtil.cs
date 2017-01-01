using System;
using Android.App;
using Android.Gms.Common;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace TigerApp.Droid.Utils
{
    public class PlatformUtil
    {
        public static bool IsGooglePlayServicesAvailable(Activity activity)
        {
            var googleApiAvailability = GoogleApiAvailability.Instance;
            var status = googleApiAvailability.IsGooglePlayServicesAvailable(activity);
            if (status != ConnectionResult.Success)
            {
                if (googleApiAvailability.IsUserResolvableError(status))
                {
                    googleApiAvailability.GetErrorDialog(activity, status, 2404).Show();
                }
                return false;
            }
            return true;
        }

        public static void UnbindDrawables(View view)
        {
            try
            {
                if (view.Background != null)
                    view.Background.SetCallback(null);

                if (view is ImageView)
                {
                    ImageView imageView = (ImageView)view;
                    if (imageView.Drawable is BitmapDrawable)
                    {
                        var bitmap = ((BitmapDrawable)imageView.Drawable).Bitmap;
                        bitmap.Dispose();
                        bitmap.Recycle();
                    }

                    imageView.SetImageBitmap(null);
                }
                else if (view is ViewGroup)
                {
                    ViewGroup viewGroup = (ViewGroup)view;
                    for (int i = 0; i < viewGroup.ChildCount; i++)
                        UnbindDrawables(viewGroup.GetChildAt(i));

                    if (!(view is AdapterView))
                        viewGroup.RemoveAllViews();
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
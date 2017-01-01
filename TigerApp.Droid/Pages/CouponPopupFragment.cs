using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.UI.Coupons;

namespace TigerApp.Droid.Pages
{
    public class CouponPopupFragment : DialogFragment
    {
        protected string _couponUrl;
        protected string _amountString;

        public CouponPopupFragment(string couponUrl,int amount = 0)
        {
            _couponUrl = couponUrl;
            _amountString = amount > 0 ? string.Format("di {0} euro!",amount) : "!";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.popup_qrview_clown, container, false);
            view.FindViewById<ImageButton>(Resource.Id.ibQRViewQuit).Click += delegate { Dismiss(); };
            view.FindViewById<QRCodeView>(Resource.Id.couponQR).SetImageURI(Android.Net.Uri.Parse(_couponUrl));
            view.FindViewById<TextView>(Resource.Id.txtQRViewAmount).Text = _amountString;
            return view;

        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            return dialog;
        }
    }
}

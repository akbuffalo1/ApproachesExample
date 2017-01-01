using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TigerApp.Droid.Pages
{
    public class LogoutPopupDialogFragment : DialogFragment
    {
        public event EventHandler OnLogout;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.popup_logout, container, false);
            var confrimLogout = view.FindViewById<Button>(Resource.Id.confirmLogout);
            var cancelLogout = view.FindViewById<TextView>(Resource.Id.cancelLogout);
           
            confrimLogout.Click += (sender, args) =>
            {
                //TODO implement log out function
                Dismiss();
                OnLogout?.Invoke(this, null);
            };
            cancelLogout.Click += (sender, args) => {
                Dismiss();
            };

            return view;
          
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            base.OnActivityCreated(savedInstanceState);
        }
    }
}
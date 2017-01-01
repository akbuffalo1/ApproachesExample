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
    public class EditProfileDialogFragment : DialogFragment
    {
        public event EventHandler OnDissmiss;

        public static EditProfileDialogFragment NewInstance()
        {
            EditProfileDialogFragment rateAppDialogFragment = new EditProfileDialogFragment();
            return rateAppDialogFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.popup_edit_profile, container, false);
            return view;
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
            OnDissmiss?.Invoke(this, EventArgs.Empty);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            WindowManagerLayoutParams layoutParams = Dialog.Window.Attributes;
            layoutParams.X = 15;
            Dialog.Window.SetGravity(GravityFlags.Right);
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            base.OnActivityCreated(savedInstanceState);
        }
    }
}
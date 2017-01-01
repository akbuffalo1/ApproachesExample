using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace TigerApp.Droid.Utils
{
    public static class DialogUtil
    {
        public static void ShowAlert(Context context, string msg, string btnText = "OK", Action onOkclick = null)
        {
            var dialog = new Dialog(context, Resource.Style.AlertStyle);
            dialog.SetContentView(Resource.Layout.alert_default_layout);

            var txtMsg = dialog.FindViewById<TextView>(Resource.Id.txtMessage);
            txtMsg.Text = msg;

            var btnOk = dialog.FindViewById<Button>(Resource.Id.dialog_close);
            btnOk.Text = btnText;

            if (onOkclick != null)
            {
                btnOk.Click += (sender, args) =>
                {
                    onOkclick.Invoke();
                    dialog.Hide();
                };
            }
            else
            {
                btnOk.Click += (sender, args) =>
                {
                    dialog.Dismiss();
                };
            }

            dialog.Show();
        }

        public static void ShowConfirmation(Context context, string msg, Action onOkclick, Action onCancelClick = null)
        {
            var dialog = new Dialog(context, Resource.Style.AlertStyle);
            dialog.SetContentView(Resource.Layout.confirmation_default_layout);

            var txtMsg = dialog.FindViewById<TextView>(Resource.Id.txtMessage);
            txtMsg.Text = msg;

            var btnOk = dialog.FindViewById<Button>(Resource.Id.btnOk);
            var btnCancel = dialog.FindViewById<Button>(Resource.Id.btnCancel);


            if (onCancelClick != null)
            {
                btnCancel.Click += (sender, args) =>
                {
                    onCancelClick.Invoke();
                    dialog.Dismiss();
                };
            }
            else
            {
                btnCancel.Click += (sender, args) =>
                {
                    dialog.Dismiss();
                };
            }

            btnOk.Click += (sender, args) =>
            {
                onOkclick.Invoke();
                dialog.Dismiss();
            };

            dialog.Show();
        }

        public static ProgressDialog ShowProgress(this Context context, bool cacelable = false)
        {
            var dialog = new ProgressDialog(context);
            dialog.Show();

            dialog.SetContentView(Resource.Layout.dialog_progress);
            dialog.SetCancelable(cacelable);

            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            return dialog;
        }

        public static ProgressDialog ShowTransparentProgress(this Context context, bool cacelable = false)
        {
            var dialog = new ProgressDialog(context, Resource.Style.TransparentProgressStyle);
            dialog.Show();

            dialog.SetContentView(Resource.Layout.dialog_progress);
            dialog.SetCancelable(cacelable);

            return dialog;
        }
    }
}
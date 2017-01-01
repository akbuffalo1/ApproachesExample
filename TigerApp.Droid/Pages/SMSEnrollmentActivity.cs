#region using

using System;
using System.Reactive.Linq;
using AD;
using AD.Plugins.Permissions;
using Android.App;
using Android.OS;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.Services.Platform.Sms;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.ViewModels;

#endregion

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/facebook_app_id")]

namespace TigerApp.Droid.Pages
{
    [Activity]
    public class SMSEnrollmentActivity : BaseReactiveActivity<ISmsEnrollmentViewModel>
    {
        public SMSEnrollmentActivity()
        {
            this.WhenActivated(dispose =>
            {
                dispose(this.WhenAnyValue(x => x.txtSmsCodeInput.Text).BindTo(this, x => x.ViewModel.VerificationCode));
                dispose(this.BindCommand(ViewModel, vm => vm.PerformSmsVerification, vc => vc.btnSmsVerify));
                dispose(
                    ViewModel.WhenAnyValue(vm => vm.SignedInWithSms)
                        .Where(show => show)
                        .Subscribe(obj => { RunOnUiThread(() => { OnSignedInWithSms(); }); }));
            });
        }

        private Button btnSmsVerify { get; set; }
        private EditText txtSmsCodeInput { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_sms_enrollment);

            btnSmsVerify = FindViewById<Button>(Resource.Id.btnSmsVerify);
            txtSmsCodeInput = FindViewById<EditText>(Resource.Id.txtSmsCodeInput);

            if (!SmsCodeReceiver.Registered)
                SmsCodeReceiver.Register(ApplicationContext);

            ShowAlertIfSmsReceiveReadDisabled();
        }

        protected override void OnStart()
        {
            if (SmsCodeReceiver.Instance != null)
            {
                SmsCodeReceiver.Instance.OnSmsCodeReceived += OnSmsCodeReceived;
                OnSmsCodeReceived(SmsCodeReceiver.Instance.LastVerificationCode);
            }
            base.OnStart();
        }

        protected override void OnResume()
        {
            if (SmsCodeReceiver.Instance != null) {
                SmsCodeReceiver.Instance.OnSmsCodeReceived += OnSmsCodeReceived;
                OnSmsCodeReceived(SmsCodeReceiver.Instance.LastVerificationCode);
            }
            base.OnResume();
        }

        protected override void OnPause()
        {
            if (SmsCodeReceiver.Instance != null) {
                SmsCodeReceiver.Instance.OnSmsCodeReceived -= OnSmsCodeReceived;
            }
            base.OnPause();
        }

        private void OnSignedInWithSms()
        {
            var loading = this.ShowProgress(true);
            SmsCodeReceiver.Unregister(ApplicationContext);
            CheckProfile();
        }

        private void OnSmsCodeReceived(string code)
        {
            txtSmsCodeInput.Text = code;
        }

        private async void ShowAlertIfSmsReceiveReadDisabled()
        {
            var perm = Resolver.Resolve<IPermissions>();
            var status = await perm.CheckPermissionStatusAsync(Permission.Sms);

            if (status != PermissionStatus.Granted)
                DialogUtil.ShowAlert(this, Resources.GetString(Resource.String.msg_enable_sms_permission), "OK",
                    OnSmsAlertOk);
        }

        private void OnSmsAlertOk()
        {
            var perm = Resolver.Resolve<IPermissions>();
            perm.RequestPermissionsAsync(Permission.Sms);
        }

        public override void OnBackPressed()
        {
            StartNewActivity(typeof(HomeActivity),TransitionWay.LR);
            Finish();
        }
    }
}
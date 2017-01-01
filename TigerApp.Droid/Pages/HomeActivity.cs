using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AD;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ReactiveUI;
using TigerApp.Droid.Services.Platform.Sms;
using TigerApp.Droid.Tutorial;
using TigerApp.Droid.UI.ToolTips;
using TigerApp.Droid.Utils;
using TigerApp.Shared;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.ViewModels;
using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/facebook_app_id")]
namespace TigerApp.Droid.Pages
{
    [Activity]
    public class HomeActivity : BaseReactiveActivity<IHomeViewModel>
    {
        public HomeActivity()
        {
            this.WhenActivated(dispose =>
             {
                 dispose(this.BindCommand(ViewModel, x => x.PerformFacebookLogin, x => x.btnFacebookLogin));
                 dispose(ViewModel.WhenAnyValue(vm => vm.ShouldShowTutorial).Where(show => show == true).Subscribe((obj) =>
                 {
                     ShowTutorial();
                 }));
                 dispose(this.BindCommand(ViewModel, x => x.PerformSmsLogin, x => x.btnSmsLogin));
                 dispose(this.WhenAnyValue(x => x.txtPhoneInput.Text).BindTo(this, x => x.ViewModel.PhoneNumber));

                dispose(ViewModel.WhenAnyValue(vm => vm.IsPhoneNumberValid).BindTo(this, x => x.btnSmsLogin.Enabled));

                 dispose(ViewModel.WhenAnyValue(vm => vm.SmsSent).Where(didSend => didSend == true).Subscribe((obj) =>
                 {
                    this.RunOnUiThread(() => {
                        if (txtPhoneInput.Text.Length > 0)
                        {
                            _progressDialog = this.ShowProgress(true);
                            if (!SmsCodeReceiver.Registered)
                                SmsCodeReceiver.Register(ApplicationContext);
                            SmsCodeReceiver.ResetLastCode();
                            StartNewActivity(typeof(SMSEnrollmentActivity), TransitionWay.RL);
                            _progressDialog.Dismiss();
                            Finish();
                        }
                    });
                 }));

                 dispose(ViewModel.WhenAnyValue(vm => vm.SignedInWithFacebook).Where(isSignedIn => isSignedIn == true).Subscribe((obj) =>
                 {
                     _progressDialog = this.ShowProgress(true);
                     CheckProfile();
                 }));
             });
        }

        private Button btnFacebookLogin { get; set; }
        private Button btnSmsLogin { get; set; }
        private EditText txtPhoneInput { get; set; }
        private ImageView btnNew { get; set; }
        private ImageView btnHomeLocation { get; set; }

        private Dialog _progressDialog;

        private ICallbackManager callbackManager { get; set; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            FacebookSdk.SdkInitialize(ApplicationContext);
            AppEventsLogger.ActivateApp(Application);

            var ioc = Resolver.Resolve<IDependencyContainer>();
            callbackManager = CallbackManagerFactory.Create();
            ioc.Register(callbackManager);



            SetContentView(Resource.Layout.activity_home);

            // Get our button from the layout resource,
            btnFacebookLogin = FindViewById<Button>(Resource.Id.btnFacebookLogin);
            btnFacebookLogin.Click += (sender, e) => {
                btnFacebookLogin.Enabled = false;
            };

            btnSmsLogin = FindViewById<Button>(Resource.Id.btnSmsLogin);
            btnSmsLogin.Click += (sender, e) =>
            {
                btnSmsLogin.Enabled = false;
                _progressDialog = this.ShowProgress(true);
            };

            txtPhoneInput = FindViewById<EditText>(Resource.Id.txtPhoneInput);
            btnNew = FindViewById<ImageView>(Resource.Id.btnNew);
            btnNew.Click += (sender, e) =>
            {
                StartNewActivity(typeof(ProductsCatalogueActivity), TransitionWay.LR);
            };
            btnHomeLocation = FindViewById<ImageView>(Resource.Id.btnHomeLocation);
            btnHomeLocation.Click += (sender, e) =>
            {
                StartNewActivity(typeof(StoreLocatorActivity), TransitionWay.RL);
            };

            OnSwipeLeft += () => { StartNewActivity(typeof(StoreLocatorActivity), TransitionWay.RL); };
            OnSwipeRight += () => { StartNewActivity(typeof(ProductsCatalogueActivity), TransitionWay.LR); };

            base.OnCreate(bundle);
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void ShowTutorial()
        {
            var tips = new List<SimpleTooltip>()
            {
                 new TooltipBuilder(this)
                {
                    AnchorView = btnSmsLogin,
                    Text = Resources.GetString(Resource.String.tut_home_s1),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),

                 new TooltipBuilder(this)
                {
                    AnchorView = btnNew,
                    Text = Resources.GetString(Resource.String.tut_home_s2),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                 new TooltipBuilder(this)
                {
                    AnchorView = btnHomeLocation,
                    Text = Resources.GetString(Resource.String.tut_home_s3),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
            };

            ShowTipsAndSetFlagWhenFinish(tips, FlagStore, Constants.Flags.HOME_TUTORIAL_SHOWN);
        }
    }
}
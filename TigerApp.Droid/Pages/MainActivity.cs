using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AD;
using AD.Droid;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.UI.ToolTips;
using TigerApp.Droid.Utils;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;

/* 
 * This is temporary Activity created for quick screen preview and ui testing at current project stage.
 * Add here link to your screen
 */

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    //    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
    public class TestActivity : BaseReactiveActivity<ITestPageViewModel>
    {
        static string TAG = nameof(TestActivity);
        protected ILogger Logger;
        private List<View> _tipViews = new List<View>();

        protected override void OnCreate(Bundle bundle)
        {
            Logger = Resolver.Resolve<ILogger>();
            _flagStorageService = AD.Resolver.Resolve<IFlagStoreService>();

            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_test);

            Button btn;

            btn = FindViewById<Button>(Resource.Id.btnHomePage);
            btn.Click += (sender, args) => { StartHome(true); };
            _tipViews.Add(btn);

            btn = FindViewById<Button>(Resource.Id.btnProductCataloguePage);
            btn.Click += (sender, args) => { StartActivity(typeof(ProductsCatalogueActivity)); };
            _tipViews.Add(btn);

            btn = FindViewById<Button>(Resource.Id.btnEnableGeolocationPage);
            btn.Click += (sender, args) => { StartActivity(typeof(EnableGeolocationActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnStoresList);
            btn.Click += (sender, args) => { StartActivity(typeof(StoresListActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnSMSEnrollment);
            btn.Click += (sender, args) => { StartActivity(typeof(SMSEnrollmentActivity)); }; _tipViews.Add(btn);

            btn = FindViewById<Button>(Resource.Id.btnNicknameEnrollment);
            btn.Click += (sender, args) => { StartActivity(typeof(NicknameEnrollmentActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnCouponNumbersWithout);
            btn.Click += (sender, args) => { StartActivity(typeof(CouponPageActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnCouponNumbersWith);
            btn.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(CouponPageActivity));
                intent.PutExtra(CouponPageActivity.IN_DATA, true);
                StartActivity(intent);
            };

            btn = FindViewById<Button>(Resource.Id.btnExpHome1); _tipViews.Add(btn);
            btn.Click += (sender, args) => { StartExpHome(1); };
            btn = FindViewById<Button>(Resource.Id.btnExpHome2);
            btn.Click += (sender, args) => { StartExpHome(2); };
            btn = FindViewById<Button>(Resource.Id.btnExpHome3);
            btn.Click += (sender, args) => { StartExpHome(3); }; _tipViews.Add(btn);
            btn = FindViewById<Button>(Resource.Id.btnExpHome4);
            btn.Click += (sender, args) => { StartExpHome(4); };
            btn = FindViewById<Button>(Resource.Id.btnExpHome5); _tipViews.Add(btn);
            btn.Click += (sender, args) => { StartExpHome(5); };

            btn = FindViewById<Button>(Resource.Id.btnProfilePage);
            btn.Click += (sender, args) => { StartActivity(typeof(ProfileActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnSettingsPage);
            btn.Click += (sender, args) => { StartActivity(typeof(SettingsActivity)); };
            _tipViews.Add(btn);

            btn = FindViewById<Button>(Resource.Id.btnCheckInMissionPage);
            btn.Click += (sender, args) => { StartActivity(typeof(CheckInMissionActivity)); };

          /*  btn = FindViewById<Button>(Resource.Id.btnScanMissionPage);
            btn.Click += (sender, args) => { StartActivity(typeof(ScanMissionActivity)); };
            _tipViews.Add(btn);*/

            btn = FindViewById<Button>(Resource.Id.btnShareMissionPage);
            btn.Click += (sender, args) => { StartActivity(typeof(ShareMissionActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnEditProfileMissionPage);
            btn.Click += (sender, args) => { StartActivity(typeof(EditProfileMissionActivity)); };
            _tipViews.Add(btn);

            btn = FindViewById<Button>(Resource.Id.btnStoreLocator);
            btn.Click += (sender, args) => { StartActivity(typeof(StoreLocatorActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnEnableNotifications);
            btn.Click += (sender, args) => { StartActivity(typeof(EnableNotificationsActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnEditProfile);
            btn.Click += (sender, args) => { StartActivity(typeof(EditProfileActivity)); };

            /*
            //Restart still dont work
            btn = FindViewById<Button>(Resource.Id.btnRestartApp);
            btn.Click += (sender, args) =>
            {
                Intent mStartActivity = new Intent(this.ApplicationContext, typeof(SplashPage));
                mStartActivity.AddFlags(ActivityFlags.ClearTop
                | ActivityFlags.ClearTask
                | ActivityFlags.NewTask);
                int mPendingIntentId = 123456;
                PendingIntent mPendingIntent = PendingIntent.GetActivity(this, mPendingIntentId, mStartActivity, PendingIntentFlags.CancelCurrent);
                AlarmManager mgr = (AlarmManager)this.GetSystemService(Context.AlarmService);
                //                mgr.Set(AlarmType.Rtc, SystemClock.CurrentThreadTimeMillis() + 500, mPendingIntent);
                mgr.Set(AlarmType.RtcWakeup, DateTime.Now.Millisecond + 1500, mPendingIntent);

                Finish();
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            };*/

            btn = FindViewById<Button>(Resource.Id.btnClearAllFlags);
            btn.Click += (sender, args) =>
            {
                var flagStore = AD.Resolver.Resolve<IFlagStoreService>();
                flagStore.UnsetAll();
            };

            btn = FindViewById<Button>(Resource.Id.btnShowTooltip);
            btn.Click += (sender, args) => { ShowTooTip(); };

            btn = FindViewById<Button>(Resource.Id.btnAvatarWall);
            btn.Click += (sender, args) => { StartActivity(typeof(WallOfAvatarActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnScanReceipt);
            btn.Click += (sender, args) => { StartActivity(typeof(ScanReceiptActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnClearTutorialFlags);
            btn.Click += (sender, args) =>
            {
                var fields = GetFieldValues(typeof(Constants.Flags));
                fields.ForEach(pair =>
                {
                    if (pair.Value.ToLower().IndexOf("tutorial") >= 0)
                        _flagStorageService.Unset(pair.Value);
                });
            };

            btn = FindViewById<Button>(Resource.Id.btnTwoCityMission);
            btn.Click += (sender, args) => { StartActivity(typeof(TwoCityMissionActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnTigerTrotterMission);
            btn.Click += (sender, args) => { StartActivity(typeof(TigerTrotterMissionActivity)); };

            btn = FindViewById<Button>(Resource.Id.btnSurveyMission);
            btn.Click += (sender, args) => { StartActivity(typeof(SurveyMissionActivity)); };

            //Scan mission
            btn = FindViewById<Button>(Resource.Id.btnScanMission1); 
            btn.Click += (sender, args) => { StartScanMission(10); };
            btn = FindViewById<Button>(Resource.Id.btnScanMission2);
            btn.Click += (sender, args) => { StartScanMission(20); };
            btn = FindViewById<Button>(Resource.Id.btnScanMission3);
            btn.Click += (sender, args) => { StartScanMission(30); }; 
        }

        private void StartScanMission(int euros)
        {
            var intent = new Intent(this.ApplicationContext, typeof(ScanMissionActivity));
            intent.PutExtra(ScanMissionActivity.EurosKey, euros);
            StartActivity(intent);
        }

        private List<GravityFlags> _gravs = new List<GravityFlags>()
        {
            GravityFlags.Bottom,
//            GravityFlags.Center,
//            GravityFlags.Start,
//            GravityFlags.End,
            GravityFlags.Top
        };

        private IFlagStoreService _flagStorageService;

        private void ShowTooTip()
        {
            var view = _tipViews[RandomUtil.RandomInt(0, _tipViews.Count - 1)];
            var grav = _gravs[RandomUtil.RandomInt(0, _gravs.Count - 1)];
            new TooltipBuilder(this)
            {
                AnchorView = view,
                Text = "asdfasdfasdf",
                Gravity = grav,
                TransparentOverlay = false,
            }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
            .Build().
            Show();

            /*   .AnchorView(view)
               .Text("asdfa sdf sdf")
                                 .Gravity(grav)
//                  .Gravity(GravityFlags.Bottom)
               .TransparentOverlay(false)
               .ContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
               .Build()
               .Show();*/
        }

        private void StartExpHome(int lev)
        {
            var intent = new Intent(this.ApplicationContext, typeof(ExpHomeActivity));
            intent.PutExtra("level", lev);
            StartActivity(intent);
        }

        private void StartHome(bool showTutorial)
        {
            var intent = new Intent(this.ApplicationContext, typeof(HomeActivity));
            intent.PutExtra("showTutorial", showTutorial);
            StartActivity(intent);
        }

        public static Dictionary<string, string> GetFieldValues(Type obj)
        {
            return obj.GetFields(BindingFlags.Public | BindingFlags.Static)
                      .Where(f => f.FieldType == typeof(string))
                      .ToDictionary(f => f.Name,
                                    f => (string)f.GetValue(null));
        }
    }
}
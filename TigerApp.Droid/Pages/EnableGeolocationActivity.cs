using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD;
using AD.Plugins.Permissions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using ReactiveUI;
using TigerApp.Shared.ViewModels;

using Android;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using TigerApp.Droid.Utils;
using Permission = Android.Content.PM.Permission;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class EnableGeolocationActivity : BaseReactiveActivity<IEnableGeolocationViewModel>
    {
        static string TAG = nameof(EnableGeolocationActivity);

        private Button _btnAuthorize;
        private ImageButton _btnStores;
        private bool _firstStart = true;

        public EnableGeolocationActivity()
        {

            this.WhenActivated((dis) =>
            {
                dis(this.BindCommand(ViewModel, x => x.RequestLocationPermission, x => x._btnAuthorize));
                dis(
                ViewModel.WhenAnyValue(x => x.PermissionStatus).Subscribe(granted =>
                {
                    if (granted == AD.Plugins.Permissions.PermissionStatus.Granted)
                    {
                        StartNewActivity(typeof(HomeActivity), TransitionWay.RL);
                        Finish();
                    }
                    else
                    {
                        Logger.Debug(TAG, "notGranted");
                        if (!_firstStart)
                            ShowAlertIfLocationDisabled();
                        _firstStart = false;
                    }
                }));
            }, this);
        }

        private async void ShowAlertIfLocationDisabled()
        {
            var perm = AD.Resolver.Resolve<IPermissions>();
            var res = await perm.ShouldShowRequestPermissionRationaleAsync(AD.Plugins.Permissions.Permission.Location);

            if (res)
                DialogUtil.ShowAlert(this, Resources.GetString(Resource.String.msg_loc_you_have_to_enable));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_enable_geolocation);

            var unsupportedRegionTextView = FindViewById<TextView>(Resource.Id.unsupportedRegionText);
            unsupportedRegionTextView.Click += (sender, args) =>
            {
                StartNewActivity(typeof(UnsupportedRegionActivity), TransitionWay.RL);
            };

            _btnAuthorize = FindViewById<Button>(Resource.Id.btnAuthorize);
            _btnStores = FindViewById<ImageButton>(Resource.Id.btnStores);

            _btnStores.Click += (sender, args) =>
            {
                StartNewActivity(typeof(StoresListActivity), TransitionWay.RL);
            };
        }
    }
}
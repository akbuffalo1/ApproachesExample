using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AD;
using AD.Plugins.CurrentActivity;
using AD.Plugins.Permissions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HockeyApp.Android;
using ReactiveUI;
using TigerApp.Droid.Services.Platform;
using TigerApp.Shared;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name", MainLauncher = false, Icon = "@mipmap/ic_launcher", Theme = "@style/SplashTheme")]
    public class SplashPage : BaseReactiveActivity<ILoadingViewModel>
    {
        static string TAG = nameof(TestActivity);
        protected ILogger Logger;

        private IPermissions _locationPermissions;
        private IFlagStoreService _flagStorageService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            /*LoginManager.Register(this, Shared.Constants.HockeyApp.AppSecret, LoginManager.LoginModeEmailPassword);
            LoginManager.VerifyLogin(this, Intent);*/

            Logger = Resolver.Resolve<ILogger>();

            _locationPermissions = AD.Resolver.Resolve<IPermissions>();
            _flagStorageService = AD.Resolver.Resolve<IFlagStoreService>();
            AD.Resolver.Resolve<ICurrentActivity>().Activity = this;
            /*var configPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/tiger_app_config.txt";
            var content = "";
            if (AD.Resolver.Resolve<AD.IFileStore>().TryReadTextFile(configPath, out content))
            {
                AD.Resolver.Resolve<ITDesAuthStore>().SetAuthData(AD.Resolver.Resolve<AD.IJsonConverter>().DeserializeObject<AD.Plugins.TripleDesAuthToken.AuthData>(content));
            }
            else {
                AD.Resolver.Resolve<IFileStore>().WriteFile(configPath,AD.Resolver.Resolve<AD.IJsonConverter>().SerializeObject(AD.Resolver.Resolve<ITDesAuthStore>().GetAuthData()));
            }*////REMOVE IT...JUST FOR TESTING
            var deviceKey = AD.Resolver.Resolve<ITDesAuthStore>().GetAuthData().DeviceKey;
            if (deviceKey != null && !deviceKey.Equals(string.Empty))
            {
                UpdateCacheAndCheckProfile();
            }else{
                (this.Application as TigerApp.Droid.Application).DeviceUpdated += (sender, e) =>
                {
                    UpdateCacheAndCheckProfile();
                };
                (this.Application as TigerApp.Droid.Application).DeviceUpdateFailed += (sender, e) =>
                {
                    var betterException = e as BetterHttpResponseException;
                    var message = e.Message;
                    if (betterException != null)
                        message = "Si è verificato un errore imprevisto sui nostri server.\nRiprova più tardi.";
                    UserError.Throw(new UserError(message));
                };
            }
        }

        private void UpdateCacheAndCheckProfile() {
            //CrashManager.Register(this);
            GoogleApiServerHandler.InitServer(this);
            ViewModel.RereshData(AD.Resolver.Resolve<ITDesAuthStore>().GetAuthData());
            CheckProfile();
        }
    }
}
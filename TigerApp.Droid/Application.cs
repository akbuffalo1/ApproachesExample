using System;
using Android.App;
using Android.Runtime;
using TigerApp.Droid.Pages;
using TigerApp.Shared;
using TigerApp.Shared.Services.API;
using UK.CO.Chrisjenx.Calligraphy;

namespace TigerApp.Droid
{
    [Application(Name = "com.byters.TigerApp.Application", Theme = "@style/AppTheme")]
    public class Application : AD.Droid.ApplicationBase<AppSetup>, Android.App.Application.IActivityLifecycleCallbacks
    {
        static string TAG = nameof(TestActivity);
        protected AD.ILogger Logger;

        private AD.Plugins.Permissions.IPermissions _locationPermissions;
        private IFlagStoreService _flagStorageService;
        public event EventHandler DeviceUpdated;
        public event EventHandler<Exception> DeviceUpdateFailed;
        /* NOTE
         * Android.App.Application.IActivityLifecycleCallbacks is not redundant
         * Without interface xamarin compiler generates wrong java class for application
         * and app throws "Java.Lang.ClassCastException: com.byters.TigerApp.Application cannot be cast to android.app.Application$ActivityLifecycleCallbacks"
         */
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Logger = AD.Resolver.Resolve<AD.ILogger>();

            _locationPermissions = AD.Resolver.Resolve<AD.Plugins.Permissions.IPermissions>();
            _flagStorageService = AD.Resolver.Resolve<IFlagStoreService>();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()                 
                 .SetFontAttrId(Resource.Attribute.font)
                 .Build());
        }

        protected override void OnUpdateDeviceSucceed(bool isFirstTimeRegistration, AD.Plugins.TripleDesAuthToken.AuthData authData)
        {
            base.OnUpdateDeviceSucceed(isFirstTimeRegistration, authData);
            if (DeviceUpdated != null)
                DeviceUpdated(this,null);
        }

        protected override void OnUpdateDeviceFailed(Exception ex)
        {
            base.OnUpdateDeviceFailed(ex);
            AD.Resolver.Resolve<AD.ITDesAuthService>().Logout();
            if (DeviceUpdateFailed != null)
                DeviceUpdateFailed(this,ex);
        }
    }
}


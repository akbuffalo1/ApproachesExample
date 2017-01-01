using AD;
using AD.Plugins.Permissions;
using System;
using TigerApp.Shared;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.ViewModels;
using UIKit;
using HockeyApp.iOS;

namespace TigerApp.iOS.Pages
{
    public partial class LoadingViewController : BaseReactiveViewController<ILoadingViewModel>
    {
        public LoadingViewController()
        {
            var networkReachability = Resolver.Resolve<INetworkReachability>();

            if (networkReachability.IsConnected)
            {
                var authService = Resolver.Resolve<ITDesAuthService>();
                var authStore = Resolver.Resolve<ITDesAuthStore>();

                bool isFirstTimeRegistration = string.IsNullOrEmpty(authStore.GetAuthData()?.DeviceKey);

                authService.UpdateDevice(
                    (AD.Plugins.TripleDesAuthToken.AuthData authData) => { OnUpdateDeviceSucceed(isFirstTimeRegistration, authData); },
                    (error) => { OnUpdateDeviceFailed(error); });

                /*var manager = BITHockeyManager.SharedHockeyManager;
                manager.Configure(Shared.Constants.HockeyApp.iOSAppId);
                manager.DisableUpdateManager = true;
                manager.StartManager();*/
            }
        }

        protected void OnUpdateDeviceFailed(Exception ex)
        {
            var authService = Resolver.Resolve<ITDesAuthService>();
            authService.Logout();
            SetRootViewControllerForUnauthorized();
        }

        protected void OnUpdateDeviceSucceed(bool isFirstTimeRegistration, AD.Plugins.TripleDesAuthToken.AuthData authData)
        {
            ViewModel.RereshData(authData);
            SetRootViewControllerWithUserProfileRequest();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            activityIndicator.StartAnimating();
        }


    }
}


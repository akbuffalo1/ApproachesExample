using System;
using AD;
using UIKit;
using TigerApp.Shared.ViewModels;
using ReactiveUI;
using Foundation;
using AD.Plugins.Permissions;
using AD.iOS;

namespace TigerApp.iOS.Pages
{
    public partial class EnableGeolocationViewController : BaseReactiveViewController<IEnableGeolocationViewModel>
    {
        public EnableGeolocationViewController()
        {
            this.WhenActivated((dis) =>
            {
                ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

                dis(this.BindCommand(ViewModel, x => x.RequestLocationPermission, x => x.authorizeRegionButton));

                dis(ViewModel.WhenAnyValue(x => x.PermissionStatus).Subscribe(granted =>
                {
                    if (granted == PermissionStatus.Granted)
                    {
                        PresentViewController(new HomeViewController(), true, null);
                    }
                    else {
                        ShowAlertIfLocationDisabled(null);
                    }
                }));
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NSObject observer = null;
            observer = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIApplicationDidBecomeActiveNotification"), (obj) =>
            {
                ShowAlertIfLocationDisabled(() =>
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
                });
            });
        }

        private async void ShowAlertIfLocationDisabled(Action ifEnabled)
        {
            if (ViewModel.AskedForLocation)
            {
                var locationPermissions = await Resolver.Resolve<IPermissions>().CheckPermissionStatusAsync(Permission.Location);

                if (locationPermissions != PermissionStatus.Granted)
                {
                    var alertWindow = UIAlertController.Create("Enable location", "You have to enable location service to use this app.", UIAlertControllerStyle.Alert);
                    alertWindow.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (l) =>
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                    }));
                    PresentViewController(alertWindow, animated: true, completionHandler: null);
                }
                else {
                    ifEnabled?.Invoke();
                }
            }
        }

        partial void OnListOfStoresClicked(Foundation.NSObject sender)
        {
            PresentViewController(new StoresViewController(), true, null);
        }

        //partial void OnAuthorizeRegionClicked(Foundation.NSObject sender)
        //{
        //    PresentViewController(new HomeViewController(), true, null);
        //}

        partial void OnInAnotherRegionClicked(Foundation.NSObject sender)
        {
            PresentViewController(new UnsupportedRegionViewController(), true, null);
        }
    }
}



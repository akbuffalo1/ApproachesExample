using AD;
using Facebook.CoreKit;
using Foundation;
using System;
using TigerApp.iOS.Pages;
using TigerApp.iOS.Pages.Profile;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models.PushNotifications;
using TigerApp.Shared.Services.API;
using UIKit;
using HockeyApp.iOS;

namespace TigerApp.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : AppDelegateBase<AppSetup>
    {
        private Services.Platform.NotificationsService NotificationsPermission;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        public override void OnActivated(UIApplication application)
        {
            Services.Platform.NotificationsService.Connect();
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Services.Platform.NotificationsService.Disconnect();
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.MakeKeyAndVisible();

            ApplicationBase<AppSetup>.CheckInitialized();
            NotificationsPermission = Resolver.Resolve<INotificationsService>() as Services.Platform.NotificationsService;

            Window.RootViewController = new LoadingViewController();

            /*
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(Shared.Constants.HockeyApp.iOSAppId);
            manager.StartManager();*/

            return ApplicationDelegate.SharedInstance.FinishedLaunching(application, launchOptions);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            NotificationsPermission.RegisteredForRemoteNotifications(application, deviceToken);
        }

        public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            NotificationsPermission.DidRegisterUserNotificationSettings(application, notificationSettings);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var notification = PushNotificationsFactory.CreateFromSource(new Utils.IOSPushNotificationsSource(userInfo));
            NotificationsPermission.DidReceiveRemoteNotification(application, userInfo, completionHandler);
            ShowNotification(notification);
        }

        public UIViewController VisibleViewController
        {
            get
            {
                return _getVisibleViewController();
            }
        }

        private UIViewController _getVisibleViewController()
        {
            var root = Window.RootViewController;
            if ((root as UINavigationController) != null)
                return (root as UINavigationController).VisibleViewController;
            else if ((root as UITabBarController) != null)
                return (root as UITabBarController).SelectedViewController;
            else
            {
                if (root.PresentedViewController != null)
                    return root.PresentedViewController;
                else
                    return root;
            }
        }

        void HandleNotification(TigerPushNotification notification)
        {
            if ((notification as PointsAddedPushNotifications) != null)
            {
                var currentViewController = VisibleViewController;
                if ((currentViewController as Pages.ExpHome.ExpHomeViewController) != null)
                {
                    InvokeOnMainThread(() =>
                    {
                        (currentViewController as Pages.ExpHome.ExpHomeViewController).RefreshUserState();
                        //(currentViewController as Pages.ExpHome.ExpHomeViewController).ViewModel.GetUserState();
                    });
                }
                else
                {
                    if ((currentViewController as Pages.MissionList.MissionListViewController) != null)
                        InvokeOnMainThread(() =>
                        {
                            (currentViewController as Pages.MissionList.MissionListViewController).ViewModel.GetMissions();
                        });
                    AD.Resolver.Resolve<IStateApiService>().GetUserState().SubscribeOnce(_ => { });
                }
            }
        }

        private void ShowBadgeNotification(BadgePushNotification notification)
        {
            AD.Resolver.Resolve<IBadgesApiService>().GetBadges(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(badges =>
            {
                var badge = badges.Find(b => b.Name.Equals(notification.BadgeName));
                if (badge != null)
                    VisibleViewController.PresentViewController(new BadgeCardViewController(badge), true, null);
            });
        }

        private void ShowLevelUpNotification(LevelUpPushNotification notification)
        {
            var profileService = AD.Resolver.Resolve<IProfileApiService>();
            profileService.GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(profile =>
            {
                var userProfile = profileService.LastLoadedProfile;//calling user profile with internet priority LastLoadedProfile is the newest user profile with avatar updated
                int level = 0;
                if (Int32.TryParse(userProfile.Level, out level))
                    VisibleViewController.PresentViewController(new LevelUpCardViewController(level, AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + userProfile.Avatar.ImageUrl), true, null);
            });
        }

        private void ShowCouponNotification(CouponPushNotification notification)
        {
            var isSpecial = (notification as SpecialCouponPushNotification) != null;
            int amount = 0;
            if (!isSpecial)
            {
                Int32.TryParse((notification as DiscountCouponPushNotification).Amount, out amount);
            }
            var couponBubble = BubbleFactory.CreateCouponBubble(notification.Message, amount, isSpecial);
            VisibleViewController.Add(couponBubble);
        }

        private void ShowCheckInSurveyNotification(CheckInSurveyPushNotification notification) {
            if (notification == null)
                return;

            var alert = new UIAlertView();
            alert.Message = notification.Message;
            alert.AddButton("Annulla");
            alert.AddButton("Vai al sondaggio");
            alert.Clicked += (sender, buttonArgs) =>
            {
                if (buttonArgs.ButtonIndex == 1)
                {
                    VisibleViewController.PresentViewController(new CheckInSurveyViewController(notification.StoreId),true,null);
                }
            };
            alert.DismissWithClickedButtonIndex(0, true);
            alert.DismissWithClickedButtonIndex(1, true);
            alert.Show();
        }

        private void ShowNotification(GCMPushNotification notification)
        {
            TigerPushNotification tigerNotification = notification as TigerPushNotification;
            if (tigerNotification != null && tigerNotification.NotificationType == TigerNotificationsType.Badge)
            {
                ShowBadgeNotification(tigerNotification as BadgePushNotification);
            }
            else if (tigerNotification != null && tigerNotification.NotificationType == TigerNotificationsType.Coupon)
            {
                ShowCouponNotification(tigerNotification as CouponPushNotification);
            }
            else if (tigerNotification != null && tigerNotification.NotificationType == TigerNotificationsType.LevelUp)
            {
                ShowLevelUpNotification(tigerNotification as LevelUpPushNotification);
            }
            else if (tigerNotification != null && tigerNotification.NotificationType == TigerNotificationsType.CheckInSurvey)
            {
                ShowCheckInSurveyNotification(tigerNotification as CheckInSurveyPushNotification);
            }
            else
            {
                var alert = new UIAlertView();
                alert.Message = notification.Message;
                alert.AddButton("OK");
                alert.Clicked += (sender, buttonArgs) =>
                {
                    if (buttonArgs.ButtonIndex == 0)
                    {
                        HandleNotification(tigerNotification);
                    }
                };
                alert.DismissWithClickedButtonIndex(0, true);
                alert.Show();
            }
        }
    }
}
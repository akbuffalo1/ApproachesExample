using AD;
using ReactiveUI;
using System;
using TigerApp.iOS.Pages;
using TigerApp.iOS.Pages.ExpHome;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS
{
    public class BaseReactiveViewController<TViewModel> : ReactiveViewController, IActivatable, IViewFor, IViewFor<TViewModel>
        where TViewModel : class, IViewModelBase
    {
        private readonly bool isNavigationBarVisible;

        public BaseReactiveViewController(bool isNavigationBarVisible = false)
        {
            this.isNavigationBarVisible = isNavigationBarVisible;
        }

        public TViewModel ViewModel
        {
            get;
            set;
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LayoutViews();

            ApplicationBase<AppSetup>.CheckInitialized();
            ViewModel = Resolver.Resolve<TViewModel>();

            if (RespondsToSelector(new ObjCRuntime.Selector("edgesForExtendedLayout")))
            {
                EdgesForExtendedLayout = UIRectEdge.None;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController?.SetNavigationBarHidden(!isNavigationBarVisible, true);
        }

        protected virtual void LayoutViews() { }

        protected bool IsPresentedBy<TViewController>() where TViewController : UIViewController
        {
            return this.PresentingViewController is TViewController;
        }

        protected void GoToExpHome(string userLevel)
        {
            PresentViewController(new ExpHomeViewController(userLevel == "2" ? ExpHomeViewController.ExpHomeLevel.L2 :
                                                            userLevel == "3" ? ExpHomeViewController.ExpHomeLevel.L3 :
                                                            userLevel == "4" ? ExpHomeViewController.ExpHomeLevel.L4 :
                                                            userLevel == "5" ? ExpHomeViewController.ExpHomeLevel.L5 :
                                                                               ExpHomeViewController.ExpHomeLevel.L1), true, null);
        }

        protected void SetRootViewControllerWithUserProfileRequest()
        {
            AD.Resolver.Resolve<IProfileApiService>().GetUserInfo(AD.Plugins.Network.Rest.Priority.Internet).Subscribe(profile =>
                {
                    SetRootViewController(profile);
                });
        }

        protected void SetRootViewController(Shared.Models.UserProfile profile)
        {
            if (profile == null || profile.Equals(Shared.Models.UserProfile.Empty))
            {
                SetRootViewControllerForUnauthorized();
            }
            else
            {
                if (profile.Avatar == null)
                {
                    PresentViewController(new WallOfAvatarViewController(), true, null);
                }
                else if (string.IsNullOrEmpty(profile.NickName))
                {
                    PresentViewController(new NicknameEnrollmentViewController(), true, null);
                }
                else
                {
                    GoToExpHome(profile.Level);
                }
            }
        }

        protected void SetRootViewControllerForUnauthorized()
        {
            var FlagStorageService = AD.Resolver.Resolve<IFlagStoreService>();
            var LocationPermissions = AD.Resolver.Resolve<AD.Plugins.Permissions.IPermissions>();
            var Window = UIApplication.SharedApplication.KeyWindow;

            FlagStorageService.ExecuteIfNotSet(Shared.Constants.Flags.ASKED_FOR_NOTIFICATIONS, () =>
            {
                PresentViewController(new EnableNotificationsViewController(), true, null);
            });

            FlagStorageService.ExecuteIfSet(Shared.Constants.Flags.ASKED_FOR_NOTIFICATIONS, async () =>
            {
                var status = await LocationPermissions.CheckPermissionStatusAsync(AD.Plugins.Permissions.Permission.Location);

                FlagStorageService.ExecuteIfSet(Shared.Constants.Flags.ASKED_FOR_LOCATION, () =>
                {
                    if (status == AD.Plugins.Permissions.PermissionStatus.Granted)
                    {
                        PresentViewController(new HomeViewController(), true, null);
                    }
                    else
                    {
                        PresentViewController(new EnableGeolocationViewController(), true, null);
                    }
                });

                FlagStorageService.ExecuteIfNotSet(Shared.Constants.Flags.ASKED_FOR_LOCATION, () =>
                {
                    PresentViewController(new EnableGeolocationViewController(), true, null);
                });
            });
        }
    }
}
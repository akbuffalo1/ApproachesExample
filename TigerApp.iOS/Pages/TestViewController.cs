using CoreLocation;
using TigerApp.iOS.Pages.CatalogueOfProducts;
using TigerApp.iOS.Pages.Coupons;
using TigerApp.iOS.Pages.EditProfile;
using TigerApp.iOS.Pages.Profile;
using TigerApp.iOS.Pages.StoreLocator;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    public partial class TestViewController : BaseReactiveViewController<ReactiveViewModel>
    {
        partial void gotoEnableNotificationsPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new EnableNotificationsViewController(), true);
        }

        partial void goto2CitiesMission(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new Pages.Missions.SurveyMission.SurveyMissionViewController(), true);
        }

        partial void gotoListOfStoresPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new StoresViewController(), true);
        }

        partial void gotoUnsupportedRegionPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new UnsupportedRegionViewController(), true);
        }

        partial void gotoEnableGeolocationPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new EnableGeolocationViewController(), true);
        }

        partial void gotoHomeTutorialPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new HomeViewController(showTutorial: true), true);
        }

        partial void gotoSmsEnrollmentPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new SmsEnrollmentViewController(), true);
        }

        partial void gotoNicknameEnrollmentPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new NicknameEnrollmentViewController(), true);
        }

        partial void goToSettingsPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new SettingsViewController(), true);
        }

        partial void gotoProfilePage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new ProfileViewController(), true);
        }

        partial void gotoExpHomeLevel1(Foundation.NSObject sender)
        {
            //NavigationController.PushViewController(new ExpHomeLevel1ViewController(showTutorial: true), true);
        }

        partial void gotoExpHomeLevel2(Foundation.NSObject sender)
        {
            //NavigationController.PushViewController(new ExpHomeLevel2ViewController(), true);
        }

        partial void gotoExpHomeLevel3(Foundation.NSObject sender)
        {
            //NavigationController.PushViewController(new ExpHomeLevel3ViewController(), true);
        }

        partial void gotoExpHomeLevel4(Foundation.NSObject sender)
        {
            //NavigationController.PushViewController(new ExpHomeLevel4ViewController(), true);
        }

        partial void gotoExpHomeLevel5(Foundation.NSObject sender)
        {
            //NavigationController.PushViewController(new ExpHomeLevel5ViewController(), true);
        }

        partial void gotoStoreLocatorPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new StoreLocatorViewController(), true);
        }

        private CLLocationManager locationManager;
        private void SetupLocationManager()
        {
            locationManager = new CLLocationManager();

            locationManager.AuthorizationChanged += (sender, args) =>
            {
                var status = CLLocationManager.Status;
                locationManager.RequestLocation();
                locationManager.StartUpdatingLocation();
                var location = locationManager.Location;
            };

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                locationManager.RequestWhenInUseAuthorization();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupLocationManager();
        }

        partial void goToCheckInMission(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new CheckInMissionViewController(), true);
        }

        partial void goToEditProfileMission(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new EditProfileMissionViewController(), true);
        }

        partial void goToScanMission(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new ScanMissionViewController(), true);
        }

        partial void goToShareMission(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new ShareMissionViewController(), true);
        }

        partial void gotoEditProfilePage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new EditProfileViewController(), true);
        }

        partial void gotoCouponsPage(Foundation.NSObject sender)
        {
            PresentViewController(new CouponsViewController(), true, null);
        }

        partial void goToCatalogueOfProducts(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new CatalogueOfProductsViewController(), true);
        }

        partial void gotoNoCouponsPage(Foundation.NSObject sender)
        {
            PresentViewController(new CouponsViewController(), true, null);
        }

        partial void goToChangeNumber(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new Pages.EditProfile.ChangeNumberFirstStepViewController(), true);
        }

        partial void gotoWallOfAvatarPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new WallOfAvatarViewController(), true);
        }

        partial void goToReceiptPage(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new ScanReceiptViewController(), true);
        }

        partial void gotoTrotterMission(Foundation.NSObject sender)
        {
            NavigationController.PushViewController(new Missions.TrotterMission.TrotterMissionViewController(), true);
        }
    }
}
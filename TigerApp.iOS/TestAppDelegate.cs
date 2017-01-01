using AD;
using Facebook.CoreKit;
using Foundation;
using TigerApp.iOS.Pages;
using UIKit;

namespace TigerApp.iOS
{
    [Register("TestAppDelegate")]
    public class TestAppDelegate : AppDelegateBase<AppSetup>
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.MakeKeyAndVisible();

            ApplicationBase<AppSetup>.CheckInitialized();

            SetRootViewController();

            return ApplicationDelegate.SharedInstance.FinishedLaunching(application, launchOptions);
        }

        private void SetRootViewController()
        {
            Window.RootViewController = new UINavigationController(new TestViewController());
        }
    }
}
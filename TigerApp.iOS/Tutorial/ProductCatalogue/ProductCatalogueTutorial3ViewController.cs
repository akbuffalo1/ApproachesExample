using System;
using TigerApp.Shared;
using UIKit;

namespace TigerApp.iOS.Tutorial.ProductCatalogue
{
    public partial class ProductCatalogueTutorial3ViewController : UIViewController
    {
        public ProductCatalogueTutorial3ViewController(Action OnDismissal) : base("ProductCatalogueTutorial3ViewController", null)
        {
            TransitioningDelegate = TransitionManager.Right;

            var swipeRight = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>
            {
                if (obj.State == UIGestureRecognizerState.Ended)
                {
                    DismissViewController(true, null);
                }
            });

            swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            swipeRight.CancelsTouchesInView = false;

            View.AddGestureRecognizer(swipeRight);

            var swipeLeft = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>
            {
                if (obj.State == UIGestureRecognizerState.Ended)
                {
                    AD.Resolver.Resolve<IFlagStoreService>().Set(Constants.Flags.PRODUCTS_CATALOGUE_TUTORIAL_SHOWN);
                    DismissViewController(false, OnDismissal);
                }
            });

            swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            swipeLeft.CancelsTouchesInView = false;

            View.AddGestureRecognizer(swipeLeft);

            var tapGesture = new UITapGestureRecognizer((obj) =>
            {
                AD.Resolver.Resolve<IFlagStoreService>().Set(Constants.Flags.PRODUCTS_CATALOGUE_TUTORIAL_SHOWN);
                DismissViewController(false, OnDismissal);
            });

            View.AddGestureRecognizer(tapGesture);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


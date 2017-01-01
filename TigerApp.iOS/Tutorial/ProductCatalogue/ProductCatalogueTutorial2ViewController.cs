using System;

using UIKit;

namespace TigerApp.iOS.Tutorial.ProductCatalogue
{
    public partial class ProductCatalogueTutorial2ViewController : UIViewController
    {
        public ProductCatalogueTutorial2ViewController(Action OnDismissal) : base("ProductCatalogueTutorial2ViewController", null)
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
                    PresentViewController(new ProductCatalogue.ProductCatalogueTutorial3ViewController(() =>
                    {
                        DismissViewController(false, OnDismissal);
                    }), true, null);
                }
            });

            swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            swipeLeft.CancelsTouchesInView = false;

            View.AddGestureRecognizer(swipeLeft);

            var tapGesture = new UITapGestureRecognizer(() =>
            {
                PresentViewController(new ProductCatalogue.ProductCatalogueTutorial3ViewController(() =>
                {
                    DismissViewController(false, OnDismissal);
                }), true, null);
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


using System;

using UIKit;

namespace TigerApp.iOS.Tutorial.ProductCatalogue
{
    public partial class ProductCatalogueTutorial1ViewController : UIViewController
    {
        public ProductCatalogueTutorial1ViewController() : base("ProductCatalogueTutorial1ViewController", null)
        {
            ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;

            var swipeLeft = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>
            {
                if (obj.State == UIGestureRecognizerState.Ended)
                {
                    PresentViewController(new ProductCatalogue.ProductCatalogueTutorial2ViewController(() =>
                    {
                        DismissViewController(false, null);
                    }), true, null);
                }
            });

            swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            swipeLeft.CancelsTouchesInView = false;

            View.AddGestureRecognizer(swipeLeft);

            var tapGesture = new UITapGestureRecognizer((obj) =>
            {
                PresentViewController(new ProductCatalogue.ProductCatalogueTutorial2ViewController(() =>
                {
                    DismissViewController(false, null);
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


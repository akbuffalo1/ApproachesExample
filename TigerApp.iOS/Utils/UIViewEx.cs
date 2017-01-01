using System.Linq;
using System;
using TigerApp.iOS.Utils;
using UIKit;

namespace TigerApp.iOS
{
    public static class UIViewEx
    {
        public static void SetText(this UIButton self, string text)
        {
            self.SetTitle(text, UIControlState.Normal);
        }

        public static void RemoveControllerFromStack(this UIViewController controller)
        {
            if (controller.NavigationController != null)
            {
                var currentStack = controller.NavigationController.ViewControllers.ToList();
                currentStack.RemoveAt(currentStack.IndexOf(controller));
                controller.NavigationController.ViewControllers = currentStack.ToArray();
            }
		}
		
        public static UISwipeGestureRecognizer LeftSwipeTo<TViewController>(this UIViewController vc)
            where TViewController : UIViewController, new()
        {
            var swipeLeft = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>
            {
                if (obj.State == UIGestureRecognizerState.Ended)
                {
                    var toVc = new TViewController();
                    vc.PresentViewController(toVc, true, null);
                }
            });
            swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            return swipeLeft;
        }

        public static UISwipeGestureRecognizer RightSwipeTo<TViewController>(this UIViewController vc)
            where TViewController : UIViewController, new()
        {
            var swipeRight = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>
            {
                if (obj.State == UIGestureRecognizerState.Ended)
                {
                    var toVc = new TViewController();
                    vc.PresentViewController(toVc, true, null);
                }
            });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            return swipeRight;
        }

        public static UISwipeGestureRecognizer UpSwipeTo<TViewController>(this UIViewController vc)
            where TViewController : UIViewController, new()
        {
            var swipeRight = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>
            {
                if (obj.State == UIGestureRecognizerState.Ended)
                {
                    var toVc = new TViewController();
                    vc.PresentViewController(toVc, true, null);
                }
            });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Up;
            return swipeRight;
        }

        public static UISwipeGestureRecognizer DownSwipeTo<TViewController>(this UIViewController vc)
            where TViewController : UIViewController, new()
        {
            var swipeRight = new UISwipeGestureRecognizer((UISwipeGestureRecognizer obj) =>
            {
                if (obj.State == UIGestureRecognizerState.Ended)
                {
                    var toVc = new TViewController();
                    vc.PresentViewController(toVc, true, null);
                }
            });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Down;
            return swipeRight;
        }
    }
}
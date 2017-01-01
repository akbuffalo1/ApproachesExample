using System;
using CoreGraphics;
using TigerApp.iOS.Pages.Profile;
using TigerApp.iOS.Pages.Coupons;
using UIKit;
using TigerApp.Shared;

namespace TigerApp.iOS.Tutorial
{
    public class CouponTutorialViewController : TutorialViewController<CouponsViewController>
    {
        public CouponTutorialViewController(CouponsViewController vc) : base(vc)
        {
        }

        public override void OnTutorialFinished()
        {
            var flagStore = AD.Resolver.Resolve<IFlagStoreService>();
            flagStore.Set(nameof(Constants.Flags.COUPON_PAGE_TUTORIAL_SHOWN));
        }

        public override void SetupTutorial(CouponsViewController parentViewController)
        {
            var couponTextFrame = parentViewController.CouponSpecialiText.Frame;
            AddTutorialStep(new TutorialStep
            {
                Text = "Ogni volta che passi\ndi livello raggiungi un\ncoupon speciale da\nbruciare in store:\ngioca le missioni Tiger\ne raggiungi le soglie\nstabilite per passare\ndi livello!",
                Point = new CGPoint(couponTextFrame.GetMidX(), couponTextFrame.GetMaxY()),
                Orientation = TutorialBubbleOrientation.TOP_LEFT
            });

            var prizeButton = parentViewController.prizeButtons[1];
            var prizeButtonFrame = prizeButton.Frame;
            prizeButtonFrame = parentViewController.View.ConvertRectFromView(prizeButtonFrame, prizeButton.Superview);

            AddTutorialStep(new TutorialStep
            {
                OnPosition = (obj) =>
                {
                    var imgView = new UIImageView(UIImage.FromBundle("coupon_tutorial_diamond"));
                    imgView.TranslatesAutoresizingMaskIntoConstraints = false;

                    obj.Add(imgView);
                    imgView.RightAnchor.ConstraintEqualTo(obj.RightAnchor, -30).Active = true;
                    imgView.TopAnchor.ConstraintEqualTo(obj.BottomAnchor, -45).Active = true;
                    imgView.WidthAnchor.ConstraintEqualTo(60).Active = true;
                    imgView.HeightAnchor.ConstraintEqualTo(60).Active = true;
                },
                Text = "Ogni volta che completi\nil cerchio con i tuoi\npunti ricevi un coupon\ndi sconto da bruciare in\nstore: più ne collezioni\npiù gadagni!",
                Point = new CGPoint(prizeButtonFrame.GetMidX(), prizeButtonFrame.GetMinY()),
                Orientation = TutorialBubbleOrientation.BOTTOM_RIGHT
            });
        }
    }
}
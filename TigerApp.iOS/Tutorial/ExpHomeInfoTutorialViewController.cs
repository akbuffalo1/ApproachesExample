using CoreGraphics;
using System.Linq;
using TigerApp.iOS.Pages.ExpHome;
using UIKit;

namespace TigerApp.iOS.Tutorial
{
    public class ExpHomeInfoTutorialViewController : TutorialViewController<ExpHomeViewController>
    {
        public ExpHomeInfoTutorialViewController(ExpHomeViewController vc) : base(vc)
        {
        }

        public override void OnTutorialFinished()
        {
        }

        public override void SetupTutorial(ExpHomeViewController parentViewController)
        {
            var progressImageViewFrame = parentViewController.ProgressImageView.Frame;
            progressImageViewFrame = parentViewController.View.ConvertRectFromView(progressImageViewFrame, parentViewController.ProgressImageView.Superview);
            AddTutorialStep(new TutorialStep
            {
                OnPosition = (obj) =>
                {
                    TransparentView.AddImageMask(progressImageViewFrame, UIImage.FromBundle("levels_gray_circle_mask"));
                },
                Text = "Racogli i punti:\nogni volta che completi\nil cerchio, ricevi un\ncoupon di sconto!",
                Point = new CGPoint(progressImageViewFrame.GetMidX(), progressImageViewFrame.GetMaxY()),
                Orientation = TutorialBubbleOrientation.TOP_LEFT
            });

            var bubble = parentViewController.ProgressBubbles.Subviews[2].Subviews[0];
            var bubbleFrame = bubble.Frame;
            bubbleFrame = parentViewController.View.ConvertRectFromView(bubbleFrame, bubble.Superview);

            var lastBubble = parentViewController.ProgressBubbles.Subviews[parentViewController.ProgressBubbles.Subviews.Length - 1];
            var lastBubbleFrame = lastBubble.Frame;
            lastBubbleFrame = parentViewController.View.ConvertRectFromView(lastBubbleFrame, lastBubble.Superview);

            AddTutorialStep(new TutorialStep
            {
                OnPosition = (obj) =>
                {
                    foreach (var v in parentViewController.ProgressBubbles.Subviews.Where(s => s != lastBubble && s.Hidden == false).Select(s => s.Subviews[0]))
                    {
                        var frm = parentViewController.View.ConvertRectFromView(v.Frame, v.Superview);
                        TransparentView.AddImageMask(frm, UIImage.FromBundle("app_usage_08_mask"));
                    }

                    var frame = parentViewController.View.ConvertRectFromView(lastBubble.Frame, lastBubble.Superview);
                    TransparentView.AddImageMask(frame, UIImage.FromBundle("app_usage_09_mask"));
                },
                Text = "Completa le missioni\ne raggiungi la soglia\nindicata per ricevere\nun coupon sopresa e\npassare di livello!\n\nOgni pallino è una\nmissione da compiere",
                Point = new CGPoint(bubbleFrame.GetMidX(), lastBubbleFrame.GetMinY()),
                Orientation = TutorialBubbleOrientation.BOTTOM_LEFT
            });

            var profileButtonFrame = parentViewController.ProfileButton.Frame;
            AddTutorialStep(new TutorialStep
            {
                Text = "Qui accedi al tuo profilo:\nscegli un nuovo avatar,\nvedi i tuoi coupon e\nscopri i badge che hai\naccumulato!",
                Point = new CGPoint(profileButtonFrame.GetMidX(), profileButtonFrame.GetMaxY()),
                Orientation = TutorialBubbleOrientation.TOP_LEFT
            });

            var barcodeButtonFrame = parentViewController.BarcodeButton.Frame;
            barcodeButtonFrame = parentViewController.View.ConvertRectFromView(barcodeButtonFrame, parentViewController.BarcodeButton.Superview);
            AddTutorialStep(new TutorialStep
            {
                OnPosition = (obj) =>
                {
                    TransparentView.AddImageMask(barcodeButtonFrame, UIImage.FromBundle("app_usage_01_mask"));
                },
                Text = "Qui carica i tuoi scontrini\nper accumulare punti.\n\nOgni euro di spesa vale\n10 punti!",
                Point = new CGPoint(barcodeButtonFrame.GetMidX(), barcodeButtonFrame.GetMinY()),
                Orientation = TutorialBubbleOrientation.BOTTOM_LEFT
            });

            var missionListButtonFrame = parentViewController.MissionListButton.Frame;
            missionListButtonFrame = parentViewController.View.ConvertRectFromView(missionListButtonFrame, parentViewController.MissionListButton.Superview);
            AddTutorialStep(new TutorialStep
            {
                Text = "Qui scopri le missioni\nTiger e i punti che\npuoi accumulare!",
                Point = new CGPoint(missionListButtonFrame.GetMidX(), missionListButtonFrame.GetMinY()),
                Orientation = TutorialBubbleOrientation.BOTTOM_RIGHT
            });

            var productCatalogueButtonFrame = parentViewController.ProductCatalogueButton.Frame;
            productCatalogueButtonFrame = parentViewController.View.ConvertRectFromView(productCatalogueButtonFrame, parentViewController.ProductCatalogueButton.Superview);
            AddTutorialStep(new TutorialStep
            {
                Text = "Scopri le ultime novità\nTiger e vota i nuovi\nprodotti a catalogo, ogni\nvoto vale un punto!",
                Point = new CGPoint(productCatalogueButtonFrame.GetMaxX(), productCatalogueButtonFrame.GetMinY()),
                Orientation = TutorialBubbleOrientation.BOTTOM_LEFT
            });
        }
    }
}
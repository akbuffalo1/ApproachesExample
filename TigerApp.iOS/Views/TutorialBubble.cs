using Foundation;
using System;
using UIKit;
using TigerApp.iOS.Utils;
using CoreGraphics;
using AD.iOS;

namespace TigerApp.iOS.Tutorial
{
    public enum TutorialBubbleOrientation
    {
        TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT
    }

    public partial class TutorialBubble : UIView
    {
        private const string Name = nameof(TutorialBubble);
        public static readonly UINib Nib = UINib.FromName(Name, NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(Name);

        UIImageView bubbleImageView;
        UIImageView arrowImageView;

        public TutorialBubble(IntPtr handle) : base(handle) { }

        public TutorialBubble(UIViewController vc, TutorialStep step) { Initialize(vc, step); }

        private void Initialize(UIViewController vc, TutorialStep step)
        {
            View = Nib.Instantiate(this, null)[0] as UIView;
            View.TranslatesAutoresizingMaskIntoConstraints = false;
            TranslatesAutoresizingMaskIntoConstraints = false;
            Add(View);

            var bubbleImage = UIImage.FromBundle("bubble");
            var arrowImage = UIImage.FromBundle("arrow");

            bubbleImageView = new UIImageView(bubbleImage);
            arrowImageView = new UIImageView(arrowImage);
            textLabel.Text = step.Text;

            textLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(34), lineHeight: 28, textAlignment: UITextAlignment.Left);

            DeviceHelper.OnIphone5(() =>
            {
                textLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(30), lineHeight: 24, textAlignment: UITextAlignment.Left);
            });

            View.InsertSubviewBelow(bubbleImageView, textLabel);
            View.InsertSubviewBelow(arrowImageView, bubbleImageView);

            arrowImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            arrowImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            bubbleImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubbleImageView.ContentMode = UIViewContentMode.ScaleToFill;
            bubbleImageView.ContentStretch = new CGRect(0.5F, 0.5F, 0.05F, 0.05F);

            bubbleImageView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            bubbleImageView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            bubbleImageView.WidthAnchor.ConstraintEqualTo(WidthAnchor).Active = true;
            bubbleImageView.HeightAnchor.ConstraintEqualTo(HeightAnchor).Active = true;

            var pt = step.Point;

            vc.View.Add(this);

            var transform = CGAffineTransform.MakeIdentity();
            var leftMarginConstraint = LeftAnchor.ConstraintEqualTo(vc.View.LeftAnchor, 20);
            var rightMarginConstraint = RightAnchor.ConstraintEqualTo(vc.View.RightAnchor, -20);

            if (step.Orientation == TutorialBubbleOrientation.TOP_LEFT)
            {
                transform.Scale(1, -1);

                arrowImageView.WidthAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.HeightAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.LeftAnchor.ConstraintEqualTo(vc.View.LeftAnchor, pt.X).Active = true;
                arrowImageView.TopAnchor.ConstraintEqualTo(vc.View.TopAnchor, pt.Y).Active = true;

                var arrowHorizontalOffset = LeftAnchor.ConstraintEqualTo(arrowImageView.LeftAnchor, -10);
                arrowHorizontalOffset.Priority = 998;
                rightMarginConstraint.Priority = 999;
                arrowHorizontalOffset.Active = true;

                TopAnchor.ConstraintEqualTo(arrowImageView.BottomAnchor, -10).Active = true;
            }
            else if (step.Orientation == TutorialBubbleOrientation.TOP_RIGHT)
            {
                transform.Scale(-1, -1);

                arrowImageView.WidthAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.HeightAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.LeftAnchor.ConstraintEqualTo(vc.View.LeftAnchor, pt.X - 30).Active = true;
                arrowImageView.TopAnchor.ConstraintEqualTo(vc.View.TopAnchor, pt.Y).Active = true;

                var arrowHorizontalOffset = RightAnchor.ConstraintEqualTo(arrowImageView.RightAnchor, 10);
                arrowHorizontalOffset.Priority = 998;
                leftMarginConstraint.Priority = 999;
                arrowHorizontalOffset.Active = true;

                TopAnchor.ConstraintEqualTo(arrowImageView.BottomAnchor, -10).Active = true;
            }
            else if (step.Orientation == TutorialBubbleOrientation.BOTTOM_RIGHT)
            {
                transform.Scale(-1, 1);

                arrowImageView.WidthAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.HeightAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.LeftAnchor.ConstraintEqualTo(vc.View.LeftAnchor, pt.X - 30).Active = true;
                arrowImageView.TopAnchor.ConstraintEqualTo(vc.View.TopAnchor, pt.Y - 30).Active = true;

                var arrowHorizontalOffset = LeftAnchor.ConstraintEqualTo(arrowImageView.LeftAnchor, -10);
                arrowHorizontalOffset.Priority = 998;
                leftMarginConstraint.Priority = 999;
                arrowHorizontalOffset.Active = true;

                BottomAnchor.ConstraintEqualTo(arrowImageView.TopAnchor, 10).Active = true;
            }
            else {
                transform.Scale(1, 1);

                arrowImageView.WidthAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.HeightAnchor.ConstraintEqualTo(30).Active = true;
                arrowImageView.LeftAnchor.ConstraintEqualTo(vc.View.LeftAnchor, pt.X).Active = true;
                arrowImageView.TopAnchor.ConstraintEqualTo(vc.View.TopAnchor, pt.Y - 30).Active = true;

                var arrowHorizontalOffset = LeftAnchor.ConstraintEqualTo(arrowImageView.LeftAnchor, -10);
                arrowHorizontalOffset.Priority = 998;
                rightMarginConstraint.Priority = 999;
                arrowHorizontalOffset.Active = true;

                BottomAnchor.ConstraintEqualTo(arrowImageView.TopAnchor, 10).Active = true;
            }

            leftMarginConstraint.Active = true;
            rightMarginConstraint.Active = true;

            bubbleImageView.Transform = transform;
            arrowImageView.Transform = transform;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            WidthAnchor.ConstraintEqualTo(View.Frame.Width).Active = true;
            HeightAnchor.ConstraintEqualTo(View.Frame.Height).Active = true;
        }
    }
}
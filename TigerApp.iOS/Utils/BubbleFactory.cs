using AD.iOS;
using Foundation;
using System;
using UIKit;

namespace TigerApp.iOS.Utils
{
    public static class BubbleFactory
    {
        public static UIView CreateCouponBubble(string message, int amount, bool isSpecial = false)
        {
            float scaleFactor = DeviceHelper.IsIphone5() ? 1 : DeviceHelper.IsIphone6() ? 1.2f : 1.3f;

            var bubble = new UIView();
            bubble.TranslatesAutoresizingMaskIntoConstraints = false;

            var bubbleImageView = new UIImageView();
            bubbleImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubbleImageView.Image = UIImage.FromBundle("mechanics_popup01");
            bubbleImageView.ContentMode = UIViewContentMode.ScaleToFill;

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center, spacing: isSpecial ? 0 : 20);

            var messageLabel = UICommon.CreateLabel(Fonts.TigerBasic.WithSize((isSpecial ? 28 : 30) * scaleFactor), UIColor.Black);
            messageLabel.AttributedText = new NSAttributedString(message,
                new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = isSpecial ? UITextAlignment.Center : UITextAlignment.Left } });

            if (!isSpecial)
            {
                var diamondsImage = UIImage.FromBundle(amount == 3 ? "mechanics_01" : amount == 8 ? "mechanics_04" : amount == 14 ? "mechanics_05" : amount == 21 ? "mechanics_06" : "mechanics_07");
                nfloat diamondsImageAspectRatio = diamondsImage.Size.Height / diamondsImage.Size.Width;

                var diamondsImageView = new UIImageView();
                diamondsImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                diamondsImageView.Image = diamondsImage;
                diamondsImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

                mainStack.AddArrangedSubview(diamondsImageView);
                mainStack.AddArrangedSubview(messageLabel);

                if (amount == 3)
                {
                    mainStack.AddConstraint(NSLayoutConstraint.Create(diamondsImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 0.4f, 0));
                    mainStack.AddConstraint(NSLayoutConstraint.Create(diamondsImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, diamondsImageView, NSLayoutAttribute.Width, diamondsImageAspectRatio, 0));
                }
                else
                {
                    mainStack.AddConstraint(NSLayoutConstraint.Create(diamondsImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 0.9f, 0));
                    mainStack.AddConstraint(NSLayoutConstraint.Create(diamondsImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, diamondsImageView, NSLayoutAttribute.Width, diamondsImageAspectRatio, 0));
                }

                mainStack.AddConstraints(new NSLayoutConstraint[]
                {
                    NSLayoutConstraint.Create(messageLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Leading, 1, 20 * scaleFactor),
                    NSLayoutConstraint.Create(messageLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -40 * scaleFactor),
                });
            }
            else
            {
                var titleLabel = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(36 * scaleFactor), UIColor.Black);
                titleLabel.AttributedText = new NSAttributedString("Complimenti Jutzu!",
                    new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center } });

                var couponImage = UIImage.FromBundle("mechanics_03");
                nfloat couponImageAspectRatio = couponImage.Size.Height / couponImage.Size.Width;

                var couponImageView = new UIImageView();
                couponImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                couponImageView.Image = couponImage;
                couponImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

                mainStack.AddArrangedSubview(titleLabel);
                mainStack.AddArrangedSubview(messageLabel);
                mainStack.AddArrangedSubview(couponImageView);

                mainStack.AddConstraints(new NSLayoutConstraint[]
                {
                    NSLayoutConstraint.Create(titleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Leading, 1, 20 * scaleFactor),
                    NSLayoutConstraint.Create(titleLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Top, 1, 20 * scaleFactor),
                    NSLayoutConstraint.Create(titleLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -40 * scaleFactor),

                    NSLayoutConstraint.Create(messageLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Leading, 1, 20 * scaleFactor),
                    NSLayoutConstraint.Create(messageLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.Bottom, 1, 10 * scaleFactor),
                    NSLayoutConstraint.Create(messageLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -40 * scaleFactor),

                    NSLayoutConstraint.Create(couponImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Leading, 1, 20 * scaleFactor),
                    NSLayoutConstraint.Create(couponImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -40 * scaleFactor),
                    NSLayoutConstraint.Create(couponImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, couponImageView, NSLayoutAttribute.Width, couponImageAspectRatio, 0)
                });
            }

            bubble.AddSubview(bubbleImageView);
            bubble.AddSubview(mainStack);

            bubble.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(bubbleImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(bubbleImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Top, 1, isSpecial ? 0 : 50 * scaleFactor),
                NSLayoutConstraint.Create(bubbleImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(bubbleImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Height, 1, isSpecial ? 0 : -40 * scaleFactor),

                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubble, NSLayoutAttribute.Bottom, 1, -40 * scaleFactor),
            });

            return bubble;
        }
    }
}

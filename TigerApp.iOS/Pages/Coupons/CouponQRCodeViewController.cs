using AD.iOS;
using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.iOS.Views;
using UIKit;

namespace TigerApp.iOS.Pages.Coupons
{
    [Register("CouponQRCodeViewController")]
    public class CouponQRCodeViewController : UIViewController
    {
        private string _couponUrl;
        private int _amount;

        public CouponQRCodeViewController(string couponUrl,int amount)
        {
            _couponUrl = couponUrl;
            _amount = amount;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            float scaleFactor = DeviceHelper.IsIphone5() ? 1 : DeviceHelper.IsIphone6() ? 1.2f : 1.3f;

            View.BackgroundColor = Colors.HexEDED74;

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center, spacing: 5 * scaleFactor);

            var xImage = UIImage.FromBundle("x_card_badge");
            nfloat xImageAspectRatio = xImage.Size.Height / xImage.Size.Width;

            var xCommand = UIButton.FromType(UIButtonType.RoundedRect);
            xCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            xCommand.TintColor = UIColor.Black;
            xCommand.SetImage(xImage, UIControlState.Normal);
            xCommand.ContentMode = UIViewContentMode.ScaleAspectFit;
            xCommand.TouchUpInside += delegate { DismissViewController(true, null); };

            var logoImage = UIImage.FromBundle("FlyingTigerLogo");
            nfloat logoImageAspectratio = logoImage.Size.Height / logoImage.Size.Width;

            var logoImageView = new UIImageView();
            logoImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            logoImageView.Image = logoImage;
            logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var centerImage = UIImage.FromBundle("img_qrcode_popup");
            nfloat centerImageAspectratio = centerImage.Size.Height / centerImage.Size.Width;

            var centerImageView = new UIImageView();
            centerImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            centerImageView.Image = centerImage;
            centerImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var message = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30 * scaleFactor), UIColor.Black);
            var attributedText = new NSMutableAttributedString(
                string.Format("Hai guadagnato un coupon di\n{0} Euro!\nusalo presentando questo\nQR code in negozio",_amount),
                                                    new UIStringAttributes
                                                    {
                                                        ParagraphStyle = new NSMutableParagraphStyle
                                                        {
                                                            LineSpacing = 1f,
                                                            LineHeightMultiple = 0.7f,
                                                            Alignment = UITextAlignment.Center
                                                        }
                                                    });
            attributedText.AddAttribute(UIStringAttributeKey.Font, Fonts.TigerBasic.WithSize(36 * scaleFactor), new NSRange(28, 8));
            message.AttributedText = attributedText;

            var messageHolder = new UIView() { TranslatesAutoresizingMaskIntoConstraints = false };
            messageHolder.AddSubview(message);

            messageHolder.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(message, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, messageHolder, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(message, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, messageHolder, NSLayoutAttribute.CenterY, 1, 0)
            });

            var qrView = new QRCodeView();
            qrView.TranslatesAutoresizingMaskIntoConstraints = false;
            qrView.Url = _couponUrl;
            qrView.WidthAnchor.ConstraintEqualTo(170 * scaleFactor).Active = true;
            qrView.HeightAnchor.ConstraintEqualTo(170 * scaleFactor).Active = true;

            mainStack.AddArrangedSubview(logoImageView);
            mainStack.AddArrangedSubview(centerImageView);
            mainStack.AddArrangedSubview(messageHolder);

            View.Add(mainStack);
            View.Add(xCommand);
            View.Add(qrView);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 25 * scaleFactor),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -10 * scaleFactor),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 22 * scaleFactor),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, xCommand, NSLayoutAttribute.Width, xImageAspectRatio, 0),

                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 0.6f, 0),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Top, 1, 40 * scaleFactor),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, logoImageView, NSLayoutAttribute.Width, logoImageAspectratio, 0),

                NSLayoutConstraint.Create(centerImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -20 * scaleFactor),
                NSLayoutConstraint.Create(centerImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Leading, 1, 10 * scaleFactor),
                NSLayoutConstraint.Create(centerImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, centerImageView, NSLayoutAttribute.Width, centerImageAspectratio, 0),

                NSLayoutConstraint.Create(messageHolder, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(qrView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.CenterX, 1, 4 * scaleFactor),
                NSLayoutConstraint.Create(qrView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.CenterY, 1, 27 * scaleFactor),
            });
        }
    }
}
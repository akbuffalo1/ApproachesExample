using Foundation;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    [Register("CheckInMissionCompleteViewController")]
    public class CheckInMissionTooFarViewController : BaseReactiveViewController<ICheckInMissionTooFarViewModel>
    {
        void CloseBtn_TouchUpInside(object sender, System.EventArgs e)
        {
            DismissViewController(true, null);
        }

        public CheckInMissionTooFarViewController()
        {
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView();

            var topStripe = new UIView() { BackgroundColor = Colors.HexF3F3F2 };

            var planet = new UIImageView();
            planet.Image = UIImage.FromBundle("checkin_05");
            planet.TranslatesAutoresizingMaskIntoConstraints = false;

            var stars = new UIImageView();
            stars.Image = UIImage.FromBundle("checkin_06");
            stars.TranslatesAutoresizingMaskIntoConstraints = false;

            var imageStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, UIStackViewDistribution.Fill, UIStackViewAlignment.Bottom, 30);
            imageStack.LayoutMargins = new UIEdgeInsets(0, 40, 0, 40);
            imageStack.LayoutMarginsRelativeArrangement = true;

            imageStack.AddArrangedSubview(new UIView { BackgroundColor = UIColor.Orange });
            imageStack.AddArrangedSubview(stars);
            imageStack.AddArrangedSubview(planet);

            var messageHolder = new UIView();
            messageHolder.TranslatesAutoresizingMaskIntoConstraints = false;

            var message = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black);
            var attributedText = new NSMutableAttributedString(Constants.Strings.CheckInMissionPageCompleteMessage,
                                                    new UIStringAttributes
                                                    {
                                                        ParagraphStyle = new NSMutableParagraphStyle
                                                        {
                                                            LineSpacing = 1f,
                                                            LineHeightMultiple = 0.7f,
                                                            Alignment = UITextAlignment.Center
                                                        }
                                                    });
            attributedText.AddAttribute(UIStringAttributeKey.Font, Fonts.TigerBasic.WithSize(36), new NSRange(0, 4));
            message.AttributedText = attributedText;

            messageHolder.AddSubview(message);

            message.CenterYAnchor.ConstraintEqualTo(messageHolder.CenterYAnchor).Active = true;

            var rocket = new UIImageView();
            rocket.Image = UIImage.FromBundle("checkin_07");
            rocket.TranslatesAutoresizingMaskIntoConstraints = false;

            var bottomStripe = new UIView() { BackgroundColor = Colors.HexF3F3F2 };

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(message);
            mainStack.AddArrangedSubview(bottomStripe);

            View.Add(mainStack);
            View.Add(imageStack);
            View.Add(rocket);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 0.185f, 0),

                NSLayoutConstraint.Create(bottomStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(bottomStripe, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 0.225f, 0),

                NSLayoutConstraint.Create(imageStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(imageStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, View.Frame.Height * 0.060f),

                NSLayoutConstraint.Create(rocket, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 15),
                NSLayoutConstraint.Create(rocket, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -15)
            });

            /* Close button */
            var closeBtn = new UIButton(UIButtonType.System);
            closeBtn.TranslatesAutoresizingMaskIntoConstraints = false;
            closeBtn.SetBackgroundImage(UIImage.FromBundle("coupon_05_button"), UIControlState.Normal);
            View.Add(closeBtn);

            closeBtn.HeightAnchor.ConstraintEqualTo(25).Active = true;
            closeBtn.WidthAnchor.ConstraintEqualTo(25).Active = true;
            closeBtn.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -8).Active = true;
            closeBtn.TopAnchor.ConstraintEqualTo(View.TopAnchor, 28).Active = true;
            closeBtn.TouchUpInside += CloseBtn_TouchUpInside;
        }
    }
}
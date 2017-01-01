using System;
using Foundation;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;
using UIKit;
using ReactiveUI;

namespace TigerApp.iOS.Pages
{
    [Register("EnableNotificationsViewController")]
    public class EnableNotificationsViewController
        : BaseReactiveViewController<IEnableNotificationsViewModel>
    {
        private UIButton _okCommand;

        public EnableNotificationsViewController() : base(isNavigationBarVisible: true)
        {
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            this.WhenActivated(dis =>
            {
                dis(this.BindCommand(ViewModel, x => x.RequestRemoteNotifications, x => x._okCommand));
                dis(ViewModel.WhenAnyValue(x => x.PermissionStatus).Subscribe(permission =>
                {
                    if ((permission == AD.Plugins.Permissions.PermissionStatus.Granted) ||
                        (permission == AD.Plugins.Permissions.PermissionStatus.Denied))
                    {
                        PresentViewController(new EnableGeolocationViewController(), true, null);
                    }
                }));
            });
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView();

            var topStripe = new UIView() { BackgroundColor = Colors.HexF6E277 };

            var heartImage = new UIImageView();
            heartImage.Image = UIImage.FromBundle("ENHeartBubble");
            heartImage.WidthAnchor.ConstraintEqualTo(99).Active = true;
            heartImage.HeightAnchor.ConstraintEqualTo(92).Active = true;

            var actionHolder = new UIView();
            var actionStack = UICommon.CreateStackView(spacing: 25);

            UIStringAttributes stringAttributes = new UIStringAttributes
            {
                Font = Fonts.TigerBasic.WithSize(32),
                ForegroundColor = UIColor.Black,
                ParagraphStyle = new NSMutableParagraphStyle() { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center },

            };
            var AttributedText = new NSMutableAttributedString(Constants.Strings.EnableNotificationsMessage, stringAttributes);
            var message = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(32), UIColor.Black, UITextAlignment.Center, UILineBreakMode.WordWrap);
            message.AttributedText = AttributedText;

            _okCommand = UICommon.CreateButton("Ok");

            actionStack.AddArrangedSubview(message);
            actionStack.AddArrangedSubview(_okCommand);

            actionHolder.AddSubview(actionStack);
            actionStack.CenterYAnchor.ConstraintEqualTo(actionHolder.CenterYAnchor).Active = true;

            var phoneImage = new UIImageView();
            phoneImage.Image = UIImage.FromBundle("ENPhone");
            phoneImage.WidthAnchor.ConstraintEqualTo(127).Active = true;
            phoneImage.HeightAnchor.ConstraintEqualTo(135).Active = true;

            var bottomStripe = new UIView() { BackgroundColor = Colors.HexF6E277 };

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(heartImage);
            mainStack.AddArrangedSubview(actionHolder);
            mainStack.AddArrangedSubview(phoneImage);
            mainStack.AddArrangedSubview(bottomStripe);

            phoneImage.Superview.BringSubviewToFront(phoneImage);

            View.Add(mainStack);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                    NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                    NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                    NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                    NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),
                    NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                    NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 0.185f, 0),
                    NSLayoutConstraint.Create(message, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 50),
                    NSLayoutConstraint.Create(message, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -100),
                    NSLayoutConstraint.Create(_okCommand, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                    NSLayoutConstraint.Create(_okCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -120),
                    NSLayoutConstraint.Create(heartImage, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 30),
                    NSLayoutConstraint.Create(heartImage, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -View.Frame.Width + 129),
                    NSLayoutConstraint.Create(heartImage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, -45.5f),
                    NSLayoutConstraint.Create(phoneImage, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, View.Frame.Width - 157),
                    NSLayoutConstraint.Create(phoneImage, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -30),
                    NSLayoutConstraint.Create(phoneImage, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bottomStripe, NSLayoutAttribute.Top, 1, 67.5f),
                    NSLayoutConstraint.Create(bottomStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                    NSLayoutConstraint.Create(bottomStripe, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 0.225f, 0),
            });
        }
    }
}
using Foundation;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    [Register("PrivacyViewController")]
    public class PrivacyViewController : BaseReactiveViewController<IViewModelBase>
    {
        private UIScrollView _scrollView;
        private UILabel _message;

        public PrivacyViewController()
        {
            TransitioningDelegate = TransitionManager.Right;
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView();

            var topStripe = new UIView { BackgroundColor = Colors.HexF8F7F5 };
            topStripe.HeightAnchor.ConstraintEqualTo(64).Active = true;

            var navStack = UICommon.CreateStackView(axis: UILayoutConstraintAxis.Horizontal, alignment: UIStackViewAlignment.Center, spacing: 5);
            navStack.LayoutMargins = new UIEdgeInsets(5, 5, 5, 5);
            navStack.LayoutMarginsRelativeArrangement = true;

            var back = UICommon.CreateBackIcon();
            back.AddGestureRecognizer(new UITapGestureRecognizer(() => { DismissViewController(true, null); }));

            var title = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(24), UIColor.Black, UITextAlignment.Center, lines: 1);
            title.Text = "informativa sulla privacy";

            var dummy = new UIView();
            dummy.WidthAnchor.ConstraintEqualTo(41).Active = true;
            dummy.HeightAnchor.ConstraintEqualTo(27).Active = true;

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(dummy);

            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.White;
            _scrollView.Bounces = false;
            _scrollView.ContentInset = new UIEdgeInsets(20, 15, 20, 15);

            _message = UICommon.CreateLabel(Fonts.FrutigerRegular.Pt18, UIColor.Black, UITextAlignment.Left, UILineBreakMode.WordWrap);
            _message.AttributedText = new NSAttributedString(Constants.Strings.PrivacyMessage,
                new UIStringAttributes { BaselineOffset = 0, ParagraphStyle = new NSMutableParagraphStyle { Alignment = UITextAlignment.Justified } });


            _scrollView.AddSubview(_message);

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(navStack);
            mainStack.AddArrangedSubview(_scrollView);


            View.Add(mainStack);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, -44),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 44),

                NSLayoutConstraint.Create(_message, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _scrollView, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_message, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -30)
            });
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            _scrollView.ContentSize = new CoreGraphics.CGSize(_message.Bounds.Width, _message.Bounds.Height);
        }
    }
}
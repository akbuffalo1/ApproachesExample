using Foundation;
using HockeyApp.iOS;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    [Register("SettingsViewController")]
    public class SettingsViewController : BaseReactiveViewController<ISettingsViewModel>
    {
        public SettingsViewController()
        {
            TransitioningDelegate = TransitionManager.Right;
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView();

            var topStripe = new UIView { BackgroundColor = Colors.HexF3F3F2 };
            topStripe.HeightAnchor.ConstraintEqualTo(64).Active = true;

            var navStack = UICommon.CreateStackView(axis: UILayoutConstraintAxis.Horizontal, alignment: UIStackViewAlignment.Center, spacing: 5);
            navStack.LayoutMargins = new UIEdgeInsets(5, 5, 5, 5);
            navStack.LayoutMarginsRelativeArrangement = true;

            var back = UICommon.CreateBackIcon();
            back.AddGestureRecognizer(new UITapGestureRecognizer(() => { DismissViewController(true, null); }));

            var title = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(30), UIColor.Black, UITextAlignment.Center, lines: 1);
            title.Text = "impostazioni";

            var dummy = new UIView();
            dummy.WidthAnchor.ConstraintEqualTo(41).Active = true;
            dummy.HeightAnchor.ConstraintEqualTo(27).Active = true;

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(dummy);

            var logoutPopUp = CreatePopUpView();

            var termsCommand = CreateButton("Regolamento / Termini di servizi");

            var privacyCommand = CreateButton("Informativa sulla privacy");
            privacyCommand.TouchUpInside += delegate { PresentViewController(new PrivacyViewController(), true, null); };

            var feedbackCommand = CreateButton("Feedback");
            feedbackCommand.TouchUpInside += delegate { 
                var feedbackManager = BITHockeyManager.SharedHockeyManager.FeedbackManager;
                feedbackManager.ShowFeedbackComposeView();
                //PresentViewController(new Settings.SettingsFeedbackViewController(), true, null); 
            };

            var logoutCommand = CreateButton("Logout");
            logoutCommand.TouchUpInside += delegate
            {
                logoutPopUp.Hidden = false;
            };

            var cactus = new UIImageView();
            cactus.Image = UIImage.FromBundle("impostazioni_01");
            cactus.WidthAnchor.ConstraintEqualTo(50).Active = true;
            cactus.HeightAnchor.ConstraintEqualTo(100).Active = true;

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(navStack);
            mainStack.AddArrangedSubview(UICommon.CreateDivider(1f, color: Colors.Hex999999));
            mainStack.AddArrangedSubview(termsCommand);
            mainStack.AddArrangedSubview(UICommon.CreateDivider(2f, color: Colors.Hex999999));
            mainStack.AddArrangedSubview(privacyCommand);
            mainStack.AddArrangedSubview(UICommon.CreateDivider(2f, color: Colors.Hex999999));
            mainStack.AddArrangedSubview(feedbackCommand);
            mainStack.AddArrangedSubview(UICommon.CreateDivider(2f, color: Colors.Hex999999));
            mainStack.AddArrangedSubview(logoutCommand);
            mainStack.AddArrangedSubview(UICommon.CreateDivider(2f, color: Colors.Hex999999));
            mainStack.AddArrangedSubview(new UIView());
            mainStack.AddArrangedSubview(cactus);

            View.AddSubview(mainStack);
            View.AddSubview(logoutPopUp);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, -44),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 44),

                NSLayoutConstraint.Create(cactus, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -40),
                NSLayoutConstraint.Create(cactus, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, View.Frame.Width - 90),
                NSLayoutConstraint.Create(cactus, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -40),

                NSLayoutConstraint.Create(logoutPopUp, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 64),
                NSLayoutConstraint.Create(logoutPopUp, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(logoutPopUp, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(logoutPopUp, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, -64),
            });
        }

        private UIButton CreateButton(string title)
        {
            var button = UIButton.FromType(UIButtonType.RoundedRect);
            button.TranslatesAutoresizingMaskIntoConstraints = false;
            button.SetTitle(title, UIControlState.Normal);
            button.TitleLabel.Font = Fonts.FrutigerRegular.WithSize(20);
            button.SetTitleColor(UIColor.Black, UIControlState.Normal);
            button.ContentEdgeInsets = new UIEdgeInsets(10, 20, 10, 20);
            button.HeightAnchor.ConstraintEqualTo(60).Active = true;
            button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            button.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
            return button;
        }

        private UIView CreatePopUpView()
        {
            var popUp = new UIView();
            popUp.TranslatesAutoresizingMaskIntoConstraints = false;
            popUp.Hidden = true;
            popUp.BackgroundColor = Colors.SemiTransparentBlack;

            var background = new UIImageView();
            background.TranslatesAutoresizingMaskIntoConstraints = false;
            background.Image = UIImage.FromBundle("impostazioni_04");

            popUp.AddSubview(background);

            var stackView = UICommon.CreateStackView();
            stackView.LayoutMargins = new UIEdgeInsets(10, 30, 10, 30);
            stackView.LayoutMarginsRelativeArrangement = true;

            var question = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black, UITextAlignment.Center);
            question.Text = "sei sicuro di voler fare il logout?";

            var logoutConfirmCommand = UICommon.CreateButton("esci");
            logoutConfirmCommand.TouchUpInside += (sender, e) =>
            {
                popUp.Hidden = true;
                AD.Resolver.Resolve<AD.ITDesAuthService>().Logout();
                DismissViewController(true, null);
            };

            var cancelCommand = UIButton.FromType(UIButtonType.RoundedRect);
            cancelCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            cancelCommand.SetTitle("Annulla", UIControlState.Normal);
            cancelCommand.TitleLabel.Font = Fonts.TigerBasic.WithSize(30);
            cancelCommand.SetTitleColor(UIColor.Black, UIControlState.Normal);
            cancelCommand.ContentEdgeInsets = new UIEdgeInsets(10, 30, 10, 30);
            cancelCommand.HeightAnchor.ConstraintEqualTo(56).Active = true;
            cancelCommand.TouchUpInside += delegate { popUp.Hidden = true; };

            stackView.AddArrangedSubview(question);
            stackView.AddArrangedSubview(logoutConfirmCommand);
            stackView.AddArrangedSubview(cancelCommand);

            popUp.AddSubview(stackView);

            popUp.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(stackView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, popUp, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(stackView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, popUp, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(stackView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, popUp, NSLayoutAttribute.Width, 0.85f, 0),

                NSLayoutConstraint.Create(background, NSLayoutAttribute.Left, NSLayoutRelation.Equal, stackView, NSLayoutAttribute.Left, 1, 0),
                NSLayoutConstraint.Create(background, NSLayoutAttribute.Top, NSLayoutRelation.Equal, stackView, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(background, NSLayoutAttribute.Right, NSLayoutRelation.Equal, stackView, NSLayoutAttribute.Right, 1, 0),
                NSLayoutConstraint.Create(background, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, stackView, NSLayoutAttribute.Bottom, 1, 0)
            });

            return popUp;
        }
    }
}
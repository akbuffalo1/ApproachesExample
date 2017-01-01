using Foundation;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.EditProfile
{
    [Register("ChangeNumberSecondStepViewController")]
    public class ChangeNumberSecondStepViewController : BaseReactiveViewController<ReactiveViewModel>
    {
        public ChangeNumberSecondStepViewController()
        {
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center);

            var topStripe = new UIView { BackgroundColor = Colors.HexF8F7F5 };
            topStripe.HeightAnchor.ConstraintEqualTo(64).Active = true;

            var navStack = UICommon.CreateStackView(axis: UILayoutConstraintAxis.Horizontal, alignment: UIStackViewAlignment.Center, spacing: 5);
            navStack.LayoutMargins = new UIEdgeInsets(5, 5, 2, 5);
            navStack.LayoutMarginsRelativeArrangement = true;

            var back = UIButton.FromType(UIButtonType.Custom);
            back.TranslatesAutoresizingMaskIntoConstraints = false;
            back.SetImage(UIImage.FromBundle("back_arrow"), UIControlState.Normal);
            back.HeightAnchor.ConstraintEqualTo(32).Active = true;
            back.WidthAnchor.ConstraintEqualTo(32).Active = true;
            back.AddGestureRecognizer(new UITapGestureRecognizer(() => { DismissViewController(true, null); }));

            var title = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(26), UIColor.Black, lines: 1);
            title.Text = "CAMBIA NUM.";

            var delimiter = new UIImageView();
            delimiter.TranslatesAutoresizingMaskIntoConstraints = false;
            delimiter.Image = UIImage.FromBundle("divisor_line");
            delimiter.ContentMode = UIViewContentMode.ScaleAspectFit;
            delimiter.HeightAnchor.ConstraintEqualTo(22).Active = true;

            var endCommand = UIButton.FromType(UIButtonType.Custom);
            endCommand.SetImage(UIImage.FromBundle("checked_icon"), UIControlState.Normal);
            endCommand.SetTitle("Fine", UIControlState.Normal);
            endCommand.Font = Fonts.TigerBasic.WithSize(30);
            endCommand.ImageEdgeInsets = new UIEdgeInsets(3, 5, 3, 66);
            endCommand.TitleEdgeInsets = new UIEdgeInsets(0, -20, 0, 0);
            endCommand.SetTitleColor(UIColor.Black, UIControlState.Normal);
            endCommand.WidthAnchor.ConstraintEqualTo(90).Active = true;
            endCommand.HeightAnchor.ConstraintEqualTo(22).Active = true;
            endCommand.TouchUpInside += delegate { DismissViewController(true, null); };

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(delimiter);
            navStack.AddArrangedSubview(endCommand);

            var oldNumberLabel = UICommon.CreateLabel(Fonts.FrutigerRegular.WithSize(20), UIColor.Black);
            oldNumberLabel.Text = "Inserisci il prefisso internazionale e il tuo vecchio numero di telefono:";

            var oldNumberStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, spacing: 20);
            oldNumberStack.LayoutMargins = new UIEdgeInsets(20, 20, 20, 20);
            oldNumberStack.LayoutMarginsRelativeArrangement = true;

            var oldPhonePrefixField = UICommon.CreateTextField();
            oldPhonePrefixField.Text = "+39";
            oldPhonePrefixField.Enabled = false;

            var oldPhoneField = UICommon.CreateTextField("vecchio numero", alignment: UITextAlignment.Left, keyboardType: UIKeyboardType.PhonePad);

            oldNumberStack.AddArrangedSubview(oldPhonePrefixField);
            oldNumberStack.AddArrangedSubview(oldPhoneField);

            var newNumberLabel = UICommon.CreateLabel(Fonts.FrutigerRegular.WithSize(20), UIColor.Black);
            newNumberLabel.Text = "Inserisci il prefisso internazionale e il tuo nuovo numero di telefono:";

            var newNumberStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, spacing: 20);
            newNumberStack.LayoutMargins = new UIEdgeInsets(20, 20, 20, 20);
            newNumberStack.LayoutMarginsRelativeArrangement = true;

            var newPhonePrefixField = UICommon.CreateTextField();
            newPhonePrefixField.Text = "+39";
            newPhonePrefixField.Enabled = false;

            var newPhoneField = UICommon.CreateTextField("nuovo numero", alignment: UITextAlignment.Left, keyboardType: UIKeyboardType.PhonePad);

            newNumberStack.AddArrangedSubview(newPhonePrefixField);
            newNumberStack.AddArrangedSubview(newPhoneField);

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(navStack);
            mainStack.AddArrangedSubview(oldNumberLabel);
            mainStack.AddArrangedSubview(oldNumberStack);
            mainStack.AddArrangedSubview(newNumberLabel);
            mainStack.AddArrangedSubview(newNumberStack);
            mainStack.AddArrangedSubview(new UIView());

            View.Add(mainStack);

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

                NSLayoutConstraint.Create(oldNumberLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, 40),
                NSLayoutConstraint.Create(oldNumberLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -40),

                NSLayoutConstraint.Create(newNumberLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, oldNumberStack, NSLayoutAttribute.Bottom, 1, 40),
                NSLayoutConstraint.Create(newNumberLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -40),

                NSLayoutConstraint.Create(oldNumberStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(newNumberStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(newPhonePrefixField, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.15f, 0),
                NSLayoutConstraint.Create(oldPhonePrefixField, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.15f, 0),
            });
        }
    }
}
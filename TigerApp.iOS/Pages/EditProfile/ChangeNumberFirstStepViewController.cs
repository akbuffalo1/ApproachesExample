using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.EditProfile
{

    [Register("ChangeNumberFirstStepViewController")]
    public class ChangeNumberFirstStepViewController : BaseReactiveViewController<ReactiveViewModel>
    {
        public ChangeNumberFirstStepViewController()
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

            var nextCommand = UIButton.FromType(UIButtonType.Custom);
            nextCommand.SetImage(UIImage.FromBundle("checked_icon"), UIControlState.Normal);
            nextCommand.SetTitle("Avanti", UIControlState.Normal);
            nextCommand.Font = Fonts.TigerBasic.WithSize(30);
            nextCommand.ImageEdgeInsets = new UIEdgeInsets(3, 5, 3, 86);
            nextCommand.TitleEdgeInsets = new UIEdgeInsets(0, -20, 0, 0);
            nextCommand.SetTitleColor(UIColor.Black, UIControlState.Normal);
            nextCommand.WidthAnchor.ConstraintEqualTo(110).Active = true;
            nextCommand.HeightAnchor.ConstraintEqualTo(22).Active = true;
            nextCommand.TouchUpInside += delegate
            {
                PresentViewController(new ChangeNumberSecondStepViewController(), true, null);
                DismissViewController(false, null);
            };

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(delimiter);
            navStack.AddArrangedSubview(nextCommand);

            var mainImg = UIImage.FromBundle("cambia_num_img");

            nfloat mainImgAspectRatio = mainImg.Size.Height / mainImg.Size.Width;

            var mainImageView = new UIImageView();
            mainImageView.Image = mainImg;
            mainImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            mainImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            var message = UICommon.CreateLabel(Fonts.FrutigerRegular.Pt20, UIColor.Black);
            message.AttributedText = new NSAttributedString("Cambiando il tuo numero di telefono transferirai tutte le info, i punti, i coupon e le impostazioni del tuo account.\n\nPrima di procedere, conferma di poter ricevere SMS o chiamate sul tuo nuovo numero.\n\nSe hai un nuovo telefono e un nuovo numero, cambia prima numero sul tuo vecchio telefono.",
                new UIStringAttributes { BaselineOffset = 0, ParagraphStyle = new NSMutableParagraphStyle { Alignment = UITextAlignment.Justified } });

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(navStack);
            mainStack.AddArrangedSubview(mainImageView);
            mainStack.AddArrangedSubview(message);
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

                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, navStack, NSLayoutAttribute.Bottom, 1, 40),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.6f, 0),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainImageView, NSLayoutAttribute.Width, mainImgAspectRatio, 0),

                NSLayoutConstraint.Create(message, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainImageView, NSLayoutAttribute.Bottom, 1, 40),
                NSLayoutConstraint.Create(message, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -40)
            });
        }
    }
}
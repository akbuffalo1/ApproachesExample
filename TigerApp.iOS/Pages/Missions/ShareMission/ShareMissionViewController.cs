using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    [Register("ShareMissionViewController")]
    public class ShareMissionViewController : BaseReactiveViewController<IShareMissionViewModel>
    {
        public ShareMissionViewController()
        {
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center);

            var topStripe = new UIView { BackgroundColor = Colors.HexF8F7F5 };
            topStripe.HeightAnchor.ConstraintEqualTo(64).Active = true;

            var navStack = UICommon.CreateStackView(axis: UILayoutConstraintAxis.Horizontal, alignment: UIStackViewAlignment.LastBaseline, spacing: 5);
            navStack.LayoutMargins = new UIEdgeInsets(5, 5, 2, 5);
            navStack.LayoutMarginsRelativeArrangement = true;

            var back = UICommon.CreateBackIcon();
            back.AddGestureRecognizer(new UITapGestureRecognizer(() => { DismissViewController(true, null); }));

            var title = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(30), UIColor.Black, UITextAlignment.Center, lines: 1);
            title.Text = Constants.Strings.ShareMissionPageTitle;

            var dummy = new UIView();
            dummy.WidthAnchor.ConstraintEqualTo(41).Active = true;
            dummy.HeightAnchor.ConstraintEqualTo(27).Active = true;

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(dummy);

            var wrapper = new UIView();
            wrapper.TranslatesAutoresizingMaskIntoConstraints = false;

            var mainImg = UIImage.FromBundle("sharefb_01");

            nfloat mainImgAspectRatio = mainImg.Size.Width / mainImg.Size.Height;

            var mainImageView = new UIImageView();
            mainImageView.Image = mainImg;
            mainImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            mainImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            var facebookImg = UIImage.FromBundle("sharefb_03");

            nfloat facebookImgAspectRatio = facebookImg.Size.Width / facebookImg.Size.Height;

            var facebookImageView = new UIImageView();
            facebookImageView.Image = facebookImg;
            facebookImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            facebookImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            var pointsImg = UIImage.FromBundle("checkin_02");

            nfloat pointsImgAspectRatio = pointsImg.Size.Width / pointsImg.Size.Height;

            var pointsImageView = new UIImageView();
            pointsImageView.Hidden = true;
            pointsImageView.Image = pointsImg;
            pointsImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            pointsImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            var imageCaption = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(26), UIColor.Black);
            imageCaption.Text = "Salvadanaio Ananas";

            wrapper.AddSubview(mainImageView);
            wrapper.AddSubview(facebookImageView);
            //wrapper.AddSubview(pointsImageView);
            wrapper.AddSubview(imageCaption);

            wrapper.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Height, 0.85f, 0),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainImageView, NSLayoutAttribute.Height, mainImgAspectRatio, 0),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.CenterY, 1, 0),

                NSLayoutConstraint.Create(facebookImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Height, 0.5f, 0),
                NSLayoutConstraint.Create(facebookImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(facebookImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Bottom, 1, 15),
                NSLayoutConstraint.Create(facebookImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, facebookImageView, NSLayoutAttribute.Height, facebookImgAspectRatio, 0),

                //NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Height, 0.6f, 0),
                //NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Trailing, 1, 0),
                //NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Bottom, 1, -10),
                //NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, pointsImageView, NSLayoutAttribute.Height, pointsImgAspectRatio, 0),

                NSLayoutConstraint.Create(imageCaption, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Bottom, 1, 20),
                NSLayoutConstraint.Create(imageCaption, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.CenterX, 1, 0),
            });

            var message = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black);
            message.AttributedText = new NSAttributedString(Constants.Strings.ShareMissionPageMessage,
                new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center } });

            var completeCommand = UICommon.CreateButton("Completa la missione");
            completeCommand.TouchUpInside += delegate { PresentViewController(new CatalogueOfProducts.CatalogueOfProductsViewController(), true, null); };

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(navStack);
            mainStack.AddArrangedSubview(wrapper);
            mainStack.AddArrangedSubview(message);
            mainStack.AddArrangedSubview(completeCommand);
            mainStack.AddArrangedSubview(new UIView());

            View.Add(mainStack);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, -44),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 44),

                NSLayoutConstraint.Create(title, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, navStack, NSLayoutAttribute.Bottom, 1, 8),

                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Top, NSLayoutRelation.Equal, navStack, NSLayoutAttribute.Bottom, 1, 10),
                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 0.42f, 0),

                NSLayoutConstraint.Create(message, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(completeCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -10),
                NSLayoutConstraint.Create(completeCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -20),
            });

            var piggie = new PigTailView();
            View.Add(piggie);

            piggie.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            piggie.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            piggie.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            piggie.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            piggie.LoadPath();
            piggie.ToggleControls();
            piggie.CreatePath();
            piggie.AnimatePathReveal();
        }
    }
}
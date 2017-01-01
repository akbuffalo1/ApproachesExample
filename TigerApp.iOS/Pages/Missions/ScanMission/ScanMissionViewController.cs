using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    [Register("ScanMissionViewController")]
    public class ScanMissionViewController : BaseReactiveViewController<IScanMissionViewModel>
    {
        private readonly Euros _euros;

        public enum Euros
        {
            E10,
            E20,
            E30
        }

        public ScanMissionViewController(Euros euros = Euros.E10)
        {
            _euros = euros;
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
            title.Text = Constants.Strings.ScanMissionPageTitle;

            var dummy = new UIView();
            dummy.WidthAnchor.ConstraintEqualTo(41).Active = true;
            dummy.HeightAnchor.ConstraintEqualTo(27).Active = true;

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(dummy);

            var wrapper = new UIView();
            wrapper.TranslatesAutoresizingMaskIntoConstraints = false;

            var mainImg = UIImage.FromBundle(_euros == Euros.E10 ? "scan_scont_01" : _euros == Euros.E20 ? "scan_scontrino_20Euro" : "scan_scontrino_30Euro");

            nfloat mainImgAspectRatio = mainImg.Size.Height / mainImg.Size.Width;

            var mainImageView = new UIImageView();
            mainImageView.Image = mainImg;
            mainImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            mainImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            var pointsImg = UIImage.FromBundle(_euros == Euros.E10 ? "checkin_02" : _euros == Euros.E20 ? "scan_scontrino_20Euro_punti" : "scan_scontrino_30Euro_punti");

            nfloat pointsImgAspectRatio = pointsImg.Size.Width / pointsImg.Size.Height;

            var pointsImageView = new UIImageView();
            pointsImageView.Image = pointsImg;
            pointsImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            pointsImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            wrapper.AddSubview(mainImageView);
            wrapper.AddSubview(pointsImageView);

            wrapper.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainImageView, NSLayoutAttribute.Height, 1, 20),

                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Left, 1, 15),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Top, 1, 20),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Width, 1, -50),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainImageView, NSLayoutAttribute.Width, mainImgAspectRatio, 0),

                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Height, 0.47f, 0),
                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Bottom, 1, -25),
                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, pointsImageView, NSLayoutAttribute.Height, pointsImgAspectRatio, 0)
            });

            var message = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black);
            message.AttributedText = new NSAttributedString(string.Format("Carica uno scontrino\nsuperiore a {0} Euro per\nguadagnare ancora piu punti!", _euros == Euros.E10 ? "10" : _euros == Euros.E20 ? "20" : "30"),
                new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center } });

            var completeCommand = UICommon.CreateButton("Completa la missione");
            completeCommand.TouchUpInside += delegate { PresentViewController(new ScanReceiptViewController(), true, null); };

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

                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Top, NSLayoutRelation.Equal, navStack, NSLayoutAttribute.Bottom, 1, 0),
                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(message, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(completeCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -10),
                NSLayoutConstraint.Create(completeCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -20),
            });
        }
    }
}
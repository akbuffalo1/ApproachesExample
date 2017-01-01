using AD.iOS;
using Facebook.ShareKit;
using Foundation;
using System;
using TigerApp.iOS.Utils;
using UIKit;

namespace TigerApp.iOS.Pages.Profile
{
    [Register("LevelUpCardViewController")]
    public class LevelUpCardViewController : UIViewController, ISharingDelegate
    {
        private int _level;
        private string _avatarUrl;

        public LevelUpCardViewController(int level, string avatarUrl)
        {
            _level = level;
            _avatarUrl = avatarUrl;
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            View.BackgroundColor = Colors.SemiTransparentBlack;
        }

        public void DidCancel(ISharing sharer) { }

        public void DidComplete(ISharing sharer, NSDictionary results) => DismissViewController(true, null);

        public void DidFail(ISharing sharer, NSError error) { }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            string cardImageName = string.Empty;
            string textImageName = string.Empty;
            switch (_level)
            {
                case 2:
                    cardImageName = "cambio-livello_2";
                    textImageName = "cambio-livello_2-text";
                    break;
                case 3:
                    cardImageName = "cambio-livello_3";
                    textImageName = "cambio-livello_3-text";
                    break;
                case 4:
                    cardImageName = "cambio-livello_4";
                    textImageName = "cambio-livello_4-text";
                    break;
                case 5:
                    cardImageName = "cambio-livello_5";
                    textImageName = "cambio-livello_5-text";
                    break;
                case 6:
                    cardImageName = "cambio-livello_6";
                    textImageName = "cambio-livello_6-text";
                    break;
            }

            var cardImage = UIImage.FromBundle(cardImageName);
            nfloat cardImageAspectRatio = cardImage.Size.Height / cardImage.Size.Width;

            var cardImageView = new UIImageView();
            cardImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            cardImageView.Image = cardImage;
            cardImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var xImage = UIImage.FromBundle("x_card_badge");
            nfloat xImageAspectRatio = xImage.Size.Height / xImage.Size.Width;

            var xCommand = UIButton.FromType(UIButtonType.RoundedRect);
            xCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            xCommand.TintColor = UIColor.Black;
            xCommand.SetImage(xImage, UIControlState.Normal);
            xCommand.ContentMode = UIViewContentMode.ScaleAspectFit;
            xCommand.TouchUpInside += delegate { DismissViewController(true, null); };

            var avatarImageView = new AD.Views.iOS.ADImageView();
            avatarImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            avatarImageView.ImageUrl = _avatarUrl;
            avatarImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var textImage = UIImage.FromBundle(textImageName);
            nfloat textImageAspectRatio = textImage.Size.Height / textImage.Size.Width;

            var textImageView = new UIImageView();
            textImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            textImageView.Image = textImage;
            textImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var shareImage = UIImage.FromBundle("arrow_card_badge");
            nfloat shareImageAspectRatio = shareImage.Size.Height / shareImage.Size.Width;

            var shareCommand = UIButton.FromType(UIButtonType.RoundedRect);
            shareCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            shareCommand.TintColor = UIColor.Black;
            shareCommand.SetImage(shareImage, UIControlState.Normal);
            shareCommand.ContentMode = UIViewContentMode.ScaleAspectFit;
            shareCommand.TouchUpInside += delegate
            {
                ShareLinkContent shareContent = new ShareLinkContent();
                shareContent.ContentTitle = string.Empty;
                shareContent.ContentDescription = string.Empty;
                shareContent.ImageURL = new NSUrl(_avatarUrl);
                shareContent.SetContentUrl(new NSUrl("http://it.flyingtiger.com/it-IT"));
                ShareDialog.Show(this, shareContent, this);
            };

            View.AddSubview(cardImageView);
            View.AddSubview(xCommand);
            View.AddSubview(shareCommand);
            View.AddSubview(avatarImageView);
            View.AddSubview(textImageView);

            float scaleFactor = DeviceHelper.IsIphone5() ? 1 : DeviceHelper.IsIphone6() ? 1.2f : 1.3f;

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -20),
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Width, cardImageAspectRatio, 0),

                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Top, 1, 10 * scaleFactor),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Trailing, 1, -45 * scaleFactor),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Width, 0.08f, 0),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, xCommand, NSLayoutAttribute.Width, xImageAspectRatio, 0),

                NSLayoutConstraint.Create(avatarImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterY, 1, -67 * scaleFactor),
                NSLayoutConstraint.Create(avatarImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterX, 1, -7 * scaleFactor),
                NSLayoutConstraint.Create(avatarImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 100 * scaleFactor),
                NSLayoutConstraint.Create(avatarImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 100 * scaleFactor),

                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterY, 1, -7 * scaleFactor),
                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterX, 1, -7 * scaleFactor),
                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Width, 0.6f, 0),
                NSLayoutConstraint.Create(textImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, textImageView, NSLayoutAttribute.Width, textImageAspectRatio, 0),

                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterY, 1, 118 * scaleFactor),
                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Width, 0.14f, 0),
                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, shareCommand, NSLayoutAttribute.Width, shareImageAspectRatio, 0),
            });
        }
    }
}
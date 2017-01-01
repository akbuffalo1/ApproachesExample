using AD.iOS;
using Facebook.ShareKit;
using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.Profile
{
    [Register("BadgeCardViewController")]
    public class BadgeCardViewController : UIViewController, ISharingDelegate
    {
        private Badge _badge;

        public BadgeCardViewController(Badge badge)
        {
            _badge = badge;
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

            string imageName = string.Empty;
            switch (_badge.Slug)
            {
                case "tiger-addicted":
                    imageName = "badge_card_tiger-addicted";
                    break;
                case "tiger-social":
                    imageName = "badge_card_tiger-social";
                    break;
                case "tiger-evangelist":
                    imageName = "badgecard_tiger-evangelist";
                    break;
                case "mr-tiger":
                    imageName = "badgecard_tiger-mrtiger";
                    break;
                case "tiger-tinder":
                    imageName = "badgecard_tiger-tinder";
                    break;
                case "tiger-collector":
                    imageName = "badge_card_tiger-collector";
                    break;
                case "tiger-trotter":
                    imageName = "badgecard_tiger-trotter";
                    break;
                case "tiger-local":
                    imageName = "badgecard_tiger-local";
                    break;
                case "tiger-weekly":
                    imageName = "badgecard_tiger-weekly";
                    break;
                case "tiger-contributor":
                    imageName = "badgecard_tiger-survey";
                    break;
            }

            var cardImage = UIImage.FromBundle(imageName);
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
                shareContent.ContentTitle = _badge.Name;
                shareContent.ContentDescription = _badge.Description;
                shareContent.ImageURL = new NSUrl(_badge.ImageUrl);
                shareContent.SetContentUrl(new NSUrl("http://it.flyingtiger.com/it-IT"));
                ShareDialog.Show(this, shareContent, this);
            };

            View.AddSubview(cardImageView);
            View.AddSubview(xCommand);
            View.AddSubview(shareCommand);

            float scaleFactor = DeviceHelper.IsIphone5() ? 1 : DeviceHelper.IsIphone6() ? 1.2f : 1.3f;

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -20),
                NSLayoutConstraint.Create(cardImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Width, cardImageAspectRatio, 0),

                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Top, 1, 10 * scaleFactor),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Trailing, 1, -30 * scaleFactor),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Width, 0.08f, 0),
                NSLayoutConstraint.Create(xCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, xCommand, NSLayoutAttribute.Width, xImageAspectRatio, 0),

                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterY, 1, 120 * scaleFactor),
                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.CenterX, 1, 10 * scaleFactor),
                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, cardImageView, NSLayoutAttribute.Width, 0.14f, 0),
                NSLayoutConstraint.Create(shareCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, shareCommand, NSLayoutAttribute.Width, shareImageAspectRatio, 0),
            });
        }
    }
}
using CoreGraphics;
using Foundation;
using System;
using System.Threading.Tasks;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.Profile
{
    public class BadgeViewCell : BaseCollectionViewCell<Badge>
    {
        private const string Name = nameof(BadgeViewCell);
        public static readonly NSString Key = new NSString(Name);
        public static readonly NSString ReusableIdentifier = (NSString)Name;

        private UIImageView _avatar;

        protected BadgeViewCell(IntPtr handle) : base(handle) { }

        [Export("initWithFrame:")]
        public BadgeViewCell(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public void Initialize()
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.Clear };

            _avatar = new UIImageView();
            _avatar.TranslatesAutoresizingMaskIntoConstraints = false;
            _avatar.ContentMode = UIViewContentMode.ScaleAspectFit;

            ContentView.AddSubview(_avatar);

            ContentView.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(_avatar, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_avatar, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_avatar, NSLayoutAttribute.Width, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Height, 1, 0),
                NSLayoutConstraint.Create(_avatar, NSLayoutAttribute.Height, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Height, 1, 0),
            });
        }

        public override void Bind(Badge badge)
        {
            if (badge.ImageUrl.Equals("no.png"))
                _avatar.Image = UIImage.FromBundle("BadgePlaceholder");
            else
                LoadBadgeImage(badge.ImageUrl);
        }

        private async Task<UIImage> LoadImage(string imageUrl)
        {
            var httpClient = new System.Net.Http.HttpClient();
            Task<byte[]> contentsTask = httpClient.GetByteArrayAsync(imageUrl);
            var contents = await contentsTask;
            return UIImage.LoadFromData(NSData.FromArray(contents));
        }

        private async void LoadBadgeImage(string imageUrl)
        {
            _avatar.Image = await LoadImage(imageUrl);
        }
    }
}

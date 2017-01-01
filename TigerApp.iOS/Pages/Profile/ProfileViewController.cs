using AD;
using AD.Views.iOS;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using TigerApp.iOS.Tutorial;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.Profile
{
    [Register("ProfileViewController")]
    public class ProfileViewController : BaseReactiveViewController<IProfileViewModel>, IUICollectionViewDelegate, IUICollectionViewDelegateFlowLayout
    {
        private BadgeCollectionSource _collectionSource;

        private UIButton _editProfileCommand;
        public UIButton EditProfileCommand => _editProfileCommand;

        private UIButton _giftCommand;
        public UIButton GiftCommand => _giftCommand;

        private ADImageView _profileImageView;
        public ADImageView ProfileImage => _profileImageView;

        private UICollectionView _collectionView;
        public UICollectionView CollectionView => _collectionView;

        private UILabel _nickNameLabel;
        private UILabel _levelLabel;

        public ProfileViewController()
        {
            TransitioningDelegate = TransitionManager.Top;

            this.WhenActivated(dis =>
            {
                dis(this.OneWayBind(ViewModel, vm => vm.Username, vc => vc._nickNameLabel.Text));
                dis(this.OneWayBind(ViewModel, vm => vm.Level, vc => vc._levelLabel.Text));

                dis(ViewModel.WhenAnyValue(vm => vm.ProfileImageUrl).Subscribe(imageUrl =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (imageUrl == ProfileViewModel.PLACEHOLDER_PROFILE_IMAGE_URL)
                        {
                            _profileImageView.Image = UIImage.FromBundle(imageUrl);
                        }
                        else
                        {
                            _profileImageView.ImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + imageUrl;
                        }
                    });
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.Badges).Where(badges => badges != null).Subscribe(badges =>
                {
                    _collectionSource.Badges.Clear();
                    _collectionSource.Badges.AddRange(badges);

                    InvokeOnMainThread(() =>
                    {
                        _collectionView.ReloadData();
                        _collectionView.LayoutIfNeeded();

                        if (ViewModel.ShouldShowTutorial)
                        {
                            PresentViewController(new ProfileTutorialViewController(this), true, null);
                        }
                    });
                }));
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center);
            mainStack.LayoutMargins = new UIEdgeInsets(55 * UICommon.ScaleFactor, 0, 40 * UICommon.ScaleFactor, 0);
            mainStack.LayoutMarginsRelativeArrangement = true;

            var giftImage = UIImage.FromBundle("profile04");
            nfloat giftImageAspectRatio = giftImage.Size.Height / giftImage.Size.Width;

            _giftCommand = UIButton.FromType(UIButtonType.RoundedRect);
            _giftCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            _giftCommand.TintColor = UIColor.Black;
            _giftCommand.SetImage(giftImage, UIControlState.Normal);
            _giftCommand.ContentMode = UIViewContentMode.ScaleAspectFit;
            _giftCommand.TouchUpInside += delegate { PresentViewController(new Coupons.CouponsViewController(), true, null); };

            var editProfileImage = UIImage.FromBundle("profile05");
            nfloat editProfileImageAspectRatio = editProfileImage.Size.Height / editProfileImage.Size.Width;

            _editProfileCommand = UIButton.FromType(UIButtonType.RoundedRect);
            _editProfileCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            _editProfileCommand.TintColor = UIColor.Black;
            _editProfileCommand.SetImage(editProfileImage, UIControlState.Normal);
            _editProfileCommand.ContentMode = UIViewContentMode.ScaleAspectFit;
            _editProfileCommand.TouchUpInside += delegate { PresentViewController(new EditProfile.EditProfileViewController(), true, null); };

            var upImage = UIImage.FromBundle("profile06");
            nfloat upImageAspectRatio = upImage.Size.Height / upImage.Size.Width;

            var upCommand = UIButton.FromType(UIButtonType.RoundedRect);
            upCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            upCommand.TintColor = UIColor.Black;
            upCommand.SetImage(upImage, UIControlState.Normal);
            upCommand.ContentMode = UIViewContentMode.ScaleAspectFit;
            upCommand.TouchUpInside += delegate { DismissViewController(true, null); };

            var placeholderImage = UIImage.FromBundle("enroll_placeholder");
            nfloat placeholderImageAspectRatio = placeholderImage.Size.Height / placeholderImage.Size.Width;

            var placeholderImageView = new UIImageView();
            placeholderImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            placeholderImageView.Image = placeholderImage;
            placeholderImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            _profileImageView = new ADImageView();
            _profileImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            _profileImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            _profileImageView.UserInteractionEnabled = true;
            _profileImageView.AddGestureRecognizer(new UITapGestureRecognizer(() => { PresentViewController(new WallOfAvatarViewController(), true, null); }));

            _nickNameLabel = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(44 * UICommon.ScaleFactor), UIColor.Black);

            _levelLabel = UICommon.CreateLabel(Fonts.FrutigerRegular.WithSize(32 * UICommon.ScaleFactor), UIColor.Black);

            var levelNameLabel = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(50 * UICommon.ScaleFactor), UIColor.Black);
            levelNameLabel.Text = "TIGER PUPPY";

            var leftWrapImage = UIImage.FromBundle("profile_text_wrap_left");
            nfloat leftWrapImageAspectRatio = leftWrapImage.Size.Height / leftWrapImage.Size.Width;

            var leftWrapImageView = new UIImageView();
            leftWrapImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            leftWrapImageView.Image = leftWrapImage;
            leftWrapImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            var rightWrapImage = UIImage.FromBundle("profile_text_wrap_right");
            nfloat rightWrapImageAspectRatio = rightWrapImage.Size.Height / rightWrapImage.Size.Width;

            var rightWrapImageView = new UIImageView();
            rightWrapImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            rightWrapImageView.Image = rightWrapImage;
            rightWrapImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            var layout = new UICollectionViewFlowLayout
            {
                SectionInset = new UIEdgeInsets(0, 8 * UICommon.ScaleFactor, 0, 8 * UICommon.ScaleFactor),
                MinimumInteritemSpacing = 0,
                MinimumLineSpacing = 5 * UICommon.ScaleFactor,
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                ItemSize = new CGSize(UIScreen.MainScreen.Bounds.Width * 0.4 - 5 * UICommon.ScaleFactor, UIScreen.MainScreen.Bounds.Width * 0.4 - 5 * UICommon.ScaleFactor)
            };

            _collectionView = new UICollectionView(UIScreen.MainScreen.Bounds, layout);
            _collectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            _collectionView.ContentSize = View.Frame.Size;
            _collectionView.Bounces = false;
            _collectionView.BackgroundColor = UIColor.Clear;
            _collectionView.RegisterClassForCell(typeof(BadgeViewCell), BadgeViewCell.ReusableIdentifier);
            _collectionSource = new BadgeCollectionSource();
            _collectionView.DataSource = _collectionSource;
            _collectionView.Delegate = this;

            mainStack.AddArrangedSubview(placeholderImageView);
            mainStack.AddArrangedSubview(_nickNameLabel);
            mainStack.AddArrangedSubview(_levelLabel);
            mainStack.AddArrangedSubview(levelNameLabel);
            mainStack.AddArrangedSubview(_collectionView);

            View.AddSubviews(new UIView[] { mainStack, _giftCommand, _editProfileCommand, upCommand, _profileImageView, leftWrapImageView, rightWrapImageView });

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(_giftCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 20 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_giftCommand, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_giftCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_giftCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _giftCommand, NSLayoutAttribute.Width, giftImageAspectRatio, 0),

                NSLayoutConstraint.Create(_editProfileCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 20 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_editProfileCommand, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_editProfileCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 30 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_editProfileCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _editProfileCommand, NSLayoutAttribute.Width, editProfileImageAspectRatio, 0),

                NSLayoutConstraint.Create(upCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(upCommand, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(upCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(upCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, upCommand, NSLayoutAttribute.Width, upImageAspectRatio, 0),

                NSLayoutConstraint.Create(placeholderImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(placeholderImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.46f, 0),
                NSLayoutConstraint.Create(placeholderImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, placeholderImageView, NSLayoutAttribute.Width, placeholderImageAspectRatio, 0),

                NSLayoutConstraint.Create(_profileImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, placeholderImageView, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(_profileImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, placeholderImageView, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(_profileImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, placeholderImageView, NSLayoutAttribute.Width, 0.6f, 0),
                NSLayoutConstraint.Create(_profileImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, placeholderImageView, NSLayoutAttribute.Height, 0.6f, 0),

                NSLayoutConstraint.Create(_nickNameLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, placeholderImageView, NSLayoutAttribute.Bottom, 1, 10 * UICommon.ScaleFactor),

                NSLayoutConstraint.Create(_levelLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _nickNameLabel, NSLayoutAttribute.Bottom, 1, 10 * UICommon.ScaleFactor),

                NSLayoutConstraint.Create(levelNameLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _levelLabel, NSLayoutAttribute.Bottom, 1, 5 * UICommon.ScaleFactor),

                NSLayoutConstraint.Create(_collectionView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, levelNameLabel, NSLayoutAttribute.Bottom, 1, 20 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_collectionView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_collectionView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.4f, 0),

                NSLayoutConstraint.Create(leftWrapImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, levelNameLabel, NSLayoutAttribute.CenterY, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(leftWrapImageView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, levelNameLabel, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(leftWrapImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.08f, 0),
                NSLayoutConstraint.Create(leftWrapImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, leftWrapImageView, NSLayoutAttribute.Width, leftWrapImageAspectRatio, 0),

                NSLayoutConstraint.Create(rightWrapImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, levelNameLabel, NSLayoutAttribute.CenterY, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(rightWrapImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, levelNameLabel, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(rightWrapImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.08f, 0),
                NSLayoutConstraint.Create(rightWrapImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, rightWrapImageView, NSLayoutAttribute.Width, rightWrapImageAspectRatio, 0),
            });
        }

        [Export("collectionView:willDisplayCell:forItemAtIndexPath:")]
        public void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, Foundation.NSIndexPath indexPath)
        {
            cell.WithType<BadgeViewCell>((c) => c.Bind(_collectionSource.Badges.ElementAt(indexPath.Row)));
        }

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var badge = _collectionSource.Badges.ElementAt(indexPath.Row);

            if (!string.IsNullOrEmpty(badge.Slug))
            {
                PresentViewController(new BadgeCardViewController(badge), true, null);
            }
        }
    }
}
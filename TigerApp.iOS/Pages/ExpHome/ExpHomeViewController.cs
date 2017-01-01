using CoreGraphics;
using Foundation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TigerApp.iOS.Pages.CatalogueOfProducts;
using TigerApp.iOS.Pages.MissionList;
using TigerApp.iOS.Pages.Profile;
using TigerApp.iOS.Pages.StoreLocator;
using TigerApp.iOS.Services.Tutorial;
using TigerApp.iOS.Tutorial;
using TigerApp.iOS.Utils;
using TigerApp.iOS.Views;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.ExpHome
{
    [Register("ExpHomeViewController")]
    public class ExpHomeViewController : BaseReactiveViewController<IExpHomeViewModel>
    {
        public enum ExpHomeLevel { L1, L2, L3, L4, L5 }

        private UserState _state;
        private ExpHomeLevel _level = ExpHomeLevel.L1;

        private string _progressBarColor;
        private UIImageView _progressImageView;
        public UIImageView ProgressImageView => _progressImageView;
        private UIStackView _bubbleStack;
        public UIView ProgressBubbles => _bubbleStack;
        private UIStackView _diamondsStack;
        private UIButton _missionsCommand;
        public UIButton MissionListButton => _missionsCommand;
        private UIButton _profileCommand;
        public UIButton ProfileButton => _profileCommand;
        private UIButton _barcodeCommand;
        public UIButton BarcodeButton => _barcodeCommand;
        private UIButton _productCommand;
        public UIButton ProductCatalogueButton => _productCommand;
        private UILabel _currentPointsLabel;
        private UILabel _levelPointsLabel;
        private UILabel _levelLabel;
        private UILabel _finalScoreLabel;
        private List<UIView> _bubbles = new List<UIView>();
        private UIImageView _bigBubbleImageView;
        private ProgressView _progressView;
        private UILabel _messageLabel;
        private UIView _colorView;
        private ActivityIndicatorView _activityIndicator;

        public ExpHomeViewController(ExpHomeLevel level = ExpHomeLevel.L1)
        {
            _level = level;

            this.WhenActivated((dis) =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.State).Subscribe(state => { _updateUserState(state); }));

                dis(ViewModel.WhenAnyValue(vm => vm.ShouldShowTutorial).Where(show => show == true).Subscribe((obj) => { 
                    PresentViewController(new ExpHomeLevel1TutorialViewController(this), true, null);
                    AD.Resolver.Resolve<IFlagStoreService>().Set(Constants.Flags.EXP_PAGE_TUTORIAL_SHOWN);
                }));
            });
        }

        private void _updateUserState(UserState state)
        {
            if (state != null)
            {
                _state = state;
               
                var currentLevel = "";

                if (_state.Current.Level.StartsWith("level-", StringComparison.CurrentCulture))
                    currentLevel = _state.Current.Level.Replace("level-", string.Empty);
                currentLevel = !currentLevel.Equals("") ? currentLevel : "1";

                _level = currentLevel == "2" ? ExpHomeLevel.L2 :
                            currentLevel == "3" ? ExpHomeLevel.L3 :
                            currentLevel == "4" ? ExpHomeLevel.L4 :
                            currentLevel == "5" ? ExpHomeLevel.L5 : ExpHomeLevel.L1;

                InvokeOnMainThread(SetValues);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _activityIndicator.Hidden = false;
            if (AD.Resolver.Resolve<AD.ITDesAuthStore>().GetAuthData().AuthProviderToken == null)
                SetRootViewControllerForUnauthorized();
            else
                ViewModel.GetUserState();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Colors.HexF8F7F5;

            _colorView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };

            #region Commands

            var infoImage = UIImage.FromBundle("exp_home_usage_03_button");
            nfloat infoImageAspectRatio = infoImage.Size.Height / infoImage.Size.Width;
            var infoCommand = UICommon.CreateIconButton(infoImage, delegate { PresentViewController(new ExpHomeInfoTutorialViewController(this), true, null); });

            var profileImage = UIImage.FromBundle("exp_home_usage_04_button");
            nfloat profileImageAspectRatio = profileImage.Size.Height / profileImage.Size.Width;
            _profileCommand = UICommon.CreateIconButton(profileImage, delegate { PresentViewController(new ProfileViewController(), true, null); });

            var settingsImage = UIImage.FromBundle("exp_home_usage_05_button");
            nfloat settingsImageAspectRatio = settingsImage.Size.Height / settingsImage.Size.Width;
            var settingsCommand = UICommon.CreateIconButton(settingsImage, delegate { PresentViewController(new SettingsViewController(), true, null); });

            var productImage = UIImage.FromBundle("exp_home_usage_06_button");
            nfloat productImageAspectRatio = productImage.Size.Height / productImage.Size.Width;
            _productCommand = UICommon.CreateIconButton(productImage, delegate { PresentViewController(new CatalogueOfProductsViewController(), true, null); });

            var missionsImage = UIImage.FromBundle("mission_15_button");
            nfloat missionsImageAspectRatio = missionsImage.Size.Height / missionsImage.Size.Width;
            _missionsCommand = UICommon.CreateIconButton(missionsImage, delegate { PresentViewController(new MissionListViewController(), true, null); });

            var storesImage = UIImage.FromBundle("exp_home_usage_07_button");
            nfloat storesImageAspectRatio = storesImage.Size.Height / storesImage.Size.Width;
            var storesCommand = UICommon.CreateIconButton(storesImage, delegate { PresentViewController(new StoreLocatorViewController(), true, null); });

            #endregion

            _levelPointsLabel = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(30 * UICommon.ScaleFactor), UIColor.Black);

            _currentPointsLabel = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(74 * UICommon.ScaleFactor), UIColor.Black);

            _levelLabel = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(36 * UICommon.ScaleFactor), UIColor.Black);

            var progressImage = UIImage.FromBundle("levels_gray_circle");
            nfloat progressImageAspectRatio = progressImage.Size.Height / progressImage.Size.Width;

            _progressImageView = new UIImageView();
            _progressImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            _progressImageView.Hidden = true;
            _progressImageView.Image = progressImage;
            _progressImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            var barcodeImage = UIImage.FromBundle("exp_home_usage_01");
            nfloat barcodeImageAspectRatio = barcodeImage.Size.Height / barcodeImage.Size.Width;
            _barcodeCommand = UIButton.FromType(UIButtonType.Custom);
            _barcodeCommand.TranslatesAutoresizingMaskIntoConstraints = false;
            _barcodeCommand.SetImage(barcodeImage, UIControlState.Normal);
            _barcodeCommand.ContentMode = UIViewContentMode.ScaleAspectFit;
            _barcodeCommand.TouchUpInside += delegate { PresentViewController(new ScanReceiptViewController(), true, null); };

            _messageLabel = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30 * UICommon.ScaleFactor), UIColor.Black);

            #region Diamonds

            _diamondsStack = UICommon.CreateStackView(spacing: 5 * UICommon.ScaleFactor);
            _diamondsStack.LayoutMargins = new UIEdgeInsets(0, 40, 0, 5);
            _diamondsStack.LayoutMarginsRelativeArrangement = true;
            _diamondsStack.Hidden = true;

            var diamond1ImageView = new UIImageView();
            diamond1ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            diamond1ImageView.Image = UIImage.FromBundle("exp_home_coupon_14");
            diamond1ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            diamond1ImageView.Hidden = true;

            var diamond2ImageView = new UIImageView();
            diamond2ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            diamond2ImageView.Image = UIImage.FromBundle("exp_home_coupon_14");
            diamond2ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            diamond2ImageView.Hidden = true;

            var diamond3ImageView = new UIImageView();
            diamond3ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            diamond3ImageView.Image = UIImage.FromBundle("exp_home_coupon_14");
            diamond3ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            diamond3ImageView.Hidden = true;

            var diamond4ImageView = new UIImageView();
            diamond4ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            diamond4ImageView.Image = UIImage.FromBundle("exp_home_coupon_14");
            diamond4ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            diamond4ImageView.Hidden = true;

            var diamond5ImageView = new UIImageView();
            diamond5ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            diamond5ImageView.Image = UIImage.FromBundle("exp_home_coupon_14");
            diamond5ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            diamond5ImageView.Hidden = true;

            _diamondsStack.AddArrangedSubview(diamond1ImageView);
            _diamondsStack.AddArrangedSubview(diamond2ImageView);
            _diamondsStack.AddArrangedSubview(diamond3ImageView);
            _diamondsStack.AddArrangedSubview(diamond4ImageView);
            _diamondsStack.AddArrangedSubview(diamond5ImageView);

            #endregion

            #region Bubbles

            _bubbleStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, UIStackViewDistribution.EqualSpacing, UIStackViewAlignment.Fill);
            _bubbleStack.LayoutMargins = new UIEdgeInsets(0, 30 * UICommon.ScaleFactor, 0, 30 * UICommon.ScaleFactor);
            _bubbleStack.LayoutMarginsRelativeArrangement = true;
            _bubbleStack.Hidden = true;

            var bubbleImage = UIImage.FromBundle("exp_home_usage_08");
            var bigBubbleImage = UIImage.FromBundle("exp_home_usage_09");

            var bubble1Wrapper = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Clear, Hidden = true };
            var bubble1ImageView = new UIImageView();
            bubble1ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubble1ImageView.Image = bubbleImage;
            bubble1ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            bubble1Wrapper.Add(bubble1ImageView);
            _bubbles.Add(bubble1Wrapper);

            var bubble2Wrapper = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Clear, Hidden = true };
            var bubble2ImageView = new UIImageView();
            bubble2ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubble2ImageView.Image = bubbleImage;
            bubble2ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            bubble2Wrapper.Add(bubble2ImageView);
            _bubbles.Add(bubble2Wrapper);

            var bubble3Wrapper = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Clear, Hidden = true };
            var bubble3ImageView = new UIImageView();
            bubble3ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubble3ImageView.Image = bubbleImage;
            bubble3ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            bubble3Wrapper.Add(bubble3ImageView);
            _bubbles.Add(bubble3Wrapper);

            var bubble4Wrapper = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Clear, Hidden = true };
            var bubble4ImageView = new UIImageView();
            bubble4ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubble4ImageView.Image = bubbleImage;
            bubble4ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            bubble4Wrapper.Add(bubble4ImageView);
            _bubbles.Add(bubble4Wrapper);

            var bubble5Wrapper = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Clear, Hidden = true };
            var bubble5ImageView = new UIImageView();
            bubble5ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubble5ImageView.Image = bubbleImage;
            bubble5ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            bubble5Wrapper.Add(bubble5ImageView);
            _bubbles.Add(bubble5Wrapper);

            var bubble6Wrapper = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Clear, Hidden = true };
            var bubble6ImageView = new UIImageView();
            bubble6ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            bubble6ImageView.Image = bubbleImage;
            bubble6ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            bubble6Wrapper.Add(bubble6ImageView);
            _bubbles.Add(bubble6Wrapper);

            _bigBubbleImageView = new UIImageView();
            _bigBubbleImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            _bigBubbleImageView.Image = bigBubbleImage;
            _bigBubbleImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            _bubbleStack.AddArrangedSubview(bubble1Wrapper);
            _bubbleStack.AddArrangedSubview(bubble2Wrapper);
            _bubbleStack.AddArrangedSubview(bubble3Wrapper);
            _bubbleStack.AddArrangedSubview(bubble4Wrapper);
            _bubbleStack.AddArrangedSubview(bubble5Wrapper);
            _bubbleStack.AddArrangedSubview(bubble6Wrapper);
            _bubbleStack.AddArrangedSubview(_bigBubbleImageView);

            #endregion

            _finalScoreLabel = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(36 * UICommon.ScaleFactor), UIColor.Black);

            _activityIndicator = new ActivityIndicatorView();

            View.AddSubviews(new UIView[] { _colorView, infoCommand, _profileCommand, settingsCommand, _productCommand,
                                            _missionsCommand, storesCommand, _progressImageView, _levelPointsLabel, _currentPointsLabel,
                                            _levelLabel, _barcodeCommand, _messageLabel, _diamondsStack, _bubbleStack, _finalScoreLabel, _activityIndicator });
            #region Constraints

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(_colorView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 58 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_colorView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_colorView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_colorView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, -58 * UICommon.ScaleFactor),

                NSLayoutConstraint.Create(infoCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 20 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(infoCommand, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(infoCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(infoCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, infoCommand, NSLayoutAttribute.Width, infoImageAspectRatio, 0),

                NSLayoutConstraint.Create(_profileCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 20 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_profileCommand, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(_profileCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_profileCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _profileCommand, NSLayoutAttribute.Width, profileImageAspectRatio, 0),

                NSLayoutConstraint.Create(settingsCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 20 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(settingsCommand, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(settingsCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(settingsCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, settingsCommand, NSLayoutAttribute.Width, settingsImageAspectRatio, 0),

                NSLayoutConstraint.Create(_productCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_productCommand, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_productCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_productCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _productCommand, NSLayoutAttribute.Width, productImageAspectRatio, 0),

                NSLayoutConstraint.Create(_missionsCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_missionsCommand, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(_missionsCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_missionsCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _missionsCommand, NSLayoutAttribute.Width, missionsImageAspectRatio, 0),

                NSLayoutConstraint.Create(storesCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(storesCommand, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -10 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(storesCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(storesCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, storesCommand, NSLayoutAttribute.Width, storesImageAspectRatio, 0),

                NSLayoutConstraint.Create(_levelPointsLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 73 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_levelPointsLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),

                NSLayoutConstraint.Create(_progressImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _levelPointsLabel, NSLayoutAttribute.Bottom, 1, -8 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_progressImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(_progressImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.5f, 0),
                NSLayoutConstraint.Create(_progressImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.Width, progressImageAspectRatio, 0),

                NSLayoutConstraint.Create(_currentPointsLabel, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(_currentPointsLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.CenterX, 1, 0),

                NSLayoutConstraint.Create(_levelLabel, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.CenterY, 1, 35 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_levelLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.CenterX, 1, 0),

                NSLayoutConstraint.Create(_barcodeCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.Bottom, 1, 20 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_barcodeCommand, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(_barcodeCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 0.20f, 0),
                NSLayoutConstraint.Create(_barcodeCommand, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _barcodeCommand, NSLayoutAttribute.Width, barcodeImageAspectRatio, 0),

                NSLayoutConstraint.Create(_messageLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _barcodeCommand, NSLayoutAttribute.Bottom, 1, 15 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_messageLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),

                NSLayoutConstraint.Create(_diamondsStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_diamondsStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _progressImageView, NSLayoutAttribute.Trailing, 1, 10 * UICommon.ScaleFactor),

                NSLayoutConstraint.Create(diamond1ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond1ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond2ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond2ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond3ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond3ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond4ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond4ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 28 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond5ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 25 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(diamond5ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 28 * UICommon.ScaleFactor),

                NSLayoutConstraint.Create(_bubbleStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _messageLabel, NSLayoutAttribute.Bottom, 1, 5 * UICommon.ScaleFactor),
                NSLayoutConstraint.Create(_bubbleStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_bubbleStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_bubbleStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 0.125f, 0),

                NSLayoutConstraint.Create(bubble1Wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.4f, 0),
                NSLayoutConstraint.Create(bubble2Wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.4f, 0),
                NSLayoutConstraint.Create(bubble3Wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.4f, 0),
                NSLayoutConstraint.Create(bubble4Wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.4f, 0),
                NSLayoutConstraint.Create(bubble5Wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.4f, 0),
                NSLayoutConstraint.Create(bubble6Wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.4f, 0),
                NSLayoutConstraint.Create(bubble1ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble1ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble2ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble2ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble3ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble3ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble4ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble4ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble5ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble5ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble6ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(bubble6ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.3f, 0),
                NSLayoutConstraint.Create(_bigBubbleImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.9f, 0),
                NSLayoutConstraint.Create(_bigBubbleImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _bubbleStack, NSLayoutAttribute.Height, 0.9f, 0),

                NSLayoutConstraint.Create(_finalScoreLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _bigBubbleImageView, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(_finalScoreLabel, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, _bigBubbleImageView, NSLayoutAttribute.CenterY, 1, 0),

                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),
            });

            #endregion
        }

        private float _lerpf(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public void RefreshUserState()
        {
            AD.Resolver.Resolve<Shared.Services.API.IStateApiService>().GetUserState().SubscribeOnce(_updateUserState);
            //DismissViewController(false, () => { GoToExpHome(_level == ExpHomeLevel.L1 ? "1" : _level == ExpHomeLevel.L2 ? "2" : _level == ExpHomeLevel.L3 ? "3" : _level == ExpHomeLevel.L4 ? "4" : "5"); });
        }

        private void SetValues()
        {
            _progressBarColor = _level == ExpHomeLevel.L1 ? "#f22222" :
                                _level == ExpHomeLevel.L2 ? "#228af2" :
                                _level == ExpHomeLevel.L3 ? "#9a00bd" :
                                _level == ExpHomeLevel.L4 ? "#f22222" :
                                                            "#ffdf29";

            _colorView.BackgroundColor = _level == ExpHomeLevel.L2 ? Colors.ColorFromHexString("#FFE345") :
                                         _level == ExpHomeLevel.L3 ? Colors.ColorFromHexString("#F5AB34") :
                                         _level == ExpHomeLevel.L4 ? Colors.ColorFromHexString("#BAE051") :
                                         _level == ExpHomeLevel.L5 ? Colors.ColorFromHexString("#B2ACE8") :
                                                                     Colors.ColorFromHexString("#75DAD0");

            _messageLabel.AttributedText = new NSAttributedString(_level == ExpHomeLevel.L1 ? Constants.Strings.ExpHomeMessageLevel1 :
                                                                  _level == ExpHomeLevel.L2 ? Constants.Strings.ExpHomeMessageLevel2 :
                                                                  _level == ExpHomeLevel.L3 ? Constants.Strings.ExpHomeMessageLevel3 :
                                                                  _level == ExpHomeLevel.L4 ? Constants.Strings.ExpHomeMessageLevel4 :
                                                                                              Constants.Strings.ExpHomeMessageLevel5,
                new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center } });

            _levelLabel.Text = $"Liv.{(_level == ExpHomeLevel.L1 ? 1 : _level == ExpHomeLevel.L2 ? 2 : _level == ExpHomeLevel.L3 ? 3 : _level == ExpHomeLevel.L4 ? 4 : 5)}";

            _levelPointsLabel.Text = _state.CouponThreshold.Points.ToString();
            _finalScoreLabel.Text = _state.LevelThreshold.Points.ToString();
            _finalScoreLabel.Font = Fonts.TigerCandy.WithSize((_level == ExpHomeLevel.L1 || _level == ExpHomeLevel.L2 ? 36 : _level == ExpHomeLevel.L3 || _level == ExpHomeLevel.L4 ? 26 : 22) * UICommon.ScaleFactor);

            _progressImageView.Hidden = false;

            var desiredImgSize = (float)ProgressImageView.Frame.Width; //this is variable
            var lineWidth = 10F * UICommon.ScaleFactor;
            var blackLineHeight = 6F * UICommon.ScaleFactor;
            var topCircleRadius = (desiredImgSize - lineWidth) / 2;

            float min = (float)(_state.Current.Points - (_state.CouponThreshold.Points - 600));
            min = min >= 0 ? min : _state.Current.Points;
            float percentage = _state.Current.Points == 0 || _state.CouponThreshold.Points == 0 ? 0 : min / (float)_state.CouponThreshold.Points;

            _progressView = new ProgressView(CGRect.Empty, radius: topCircleRadius, lineWidth: lineWidth, percentage: percentage, hexColor: _progressBarColor);
            _progressView.Frame = new CGRect(desiredImgSize / 2, desiredImgSize / 2 + blackLineHeight, desiredImgSize, desiredImgSize + blackLineHeight);
            ProgressImageView.AddSubview(_progressView);

            Observable.Interval(TimeSpan.FromMilliseconds(16)).Take(61).ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((long obj) =>
            {
                var scoreText = (int)Math.Ceiling(_lerpf(0, _state.Current.Points, obj / 60F));
                _currentPointsLabel.Text = $"{scoreText}";
            });

            var _doneBubbleImage = UIImage.FromBundle(_level == ExpHomeLevel.L1 ? "exp_home_l1_usage_10" :
                                                      _level == ExpHomeLevel.L2 ? "home_lev2_01" :
                                                      _level == ExpHomeLevel.L3 ? "home_lev3_01" :
                                                      _level == ExpHomeLevel.L4 ? "home_lev4_01" :
                                                                                  "home_lev5_01");

            var _doneBigBubbleImage = UIImage.FromBundle(_level == ExpHomeLevel.L1 ? "exp_home_l1_usage_12" :
                                                         _level == ExpHomeLevel.L2 ? "home_lev2_04" :
                                                         _level == ExpHomeLevel.L3 ? "home_lev3_03" :
                                                         _level == ExpHomeLevel.L4 ? "home_lev4_04" :
                                                                                     "home_lev5_04");

            _bubbles.Take(_level == ExpHomeLevel.L1 ? 3 : _level == ExpHomeLevel.L2 ? 4 : _level == ExpHomeLevel.L5 ? 6 : 5)
                    .ToList().ForEach(b => b.Hidden = false);
            _bubbles.Take(_state.Current.MissionsCount).ToList().ForEach(b => (b.Subviews[0] as UIImageView).Image = _doneBubbleImage);
            if (_state.Current.Points >= _state.LevelThreshold.Points)
            {
                _bigBubbleImageView.Image = _doneBigBubbleImage;
            }
            ApplyBubblesConstraints();
            _bubbleStack.Hidden = false;

            _diamondsStack.Hidden = _state.Current.CouponCount > 0 ? false : true;
            _diamondsStack.ArrangedSubviews.Take(_state.Current.CouponCount).ToList().ForEach(d => d.Hidden = false);

            _activityIndicator.Hidden = true;
        }

        private void ApplyBubblesConstraints()
        {
            float margin = 8 * UICommon.ScaleFactor;

            if (_level == ExpHomeLevel.L1)
            {
                var bubble1c = _bubbles[0].Subviews[0];
                bubble1c.CenterYAnchor.ConstraintEqualTo(_bubbles[0].CenterYAnchor).Active = true;
                bubble1c.CenterXAnchor.ConstraintEqualTo(_bubbles[0].CenterXAnchor).Active = true;
                var bubble2c = _bubbles[1].Subviews[0];
                bubble2c.CenterYAnchor.ConstraintEqualTo(_bubbles[1].CenterYAnchor).Active = true;
                bubble2c.CenterXAnchor.ConstraintEqualTo(_bubbles[1].CenterXAnchor).Active = true;
                var bubble3c = _bubbles[2].Subviews[0];
                bubble3c.CenterYAnchor.ConstraintEqualTo(_bubbles[2].CenterYAnchor).Active = true;
                bubble3c.CenterXAnchor.ConstraintEqualTo(_bubbles[2].CenterXAnchor).Active = true;
            }
            else
            {
                var bubble1t = _bubbles[0].Subviews[0];
                bubble1t.TopAnchor.ConstraintEqualTo(_bubbles[0].TopAnchor, margin).Active = true;
                bubble1t.LeadingAnchor.ConstraintEqualTo(_bubbles[0].LeadingAnchor).Active = true;
                var bubble2b = _bubbles[1].Subviews[0];
                bubble2b.BottomAnchor.ConstraintEqualTo(_bubbles[1].BottomAnchor, -margin).Active = true;
                bubble2b.LeadingAnchor.ConstraintEqualTo(_bubbles[1].LeadingAnchor).Active = true;
                var bubble3t = _bubbles[2].Subviews[0];
                bubble3t.TopAnchor.ConstraintEqualTo(_bubbles[2].TopAnchor, margin).Active = true;
                bubble3t.LeadingAnchor.ConstraintEqualTo(_bubbles[2].LeadingAnchor).Active = true;
            }

            var bubble4 = _bubbles[3].Subviews[0];
            bubble4.BottomAnchor.ConstraintEqualTo(_bubbles[3].BottomAnchor, -margin).Active = true;
            bubble4.LeadingAnchor.ConstraintEqualTo(_bubbles[3].LeadingAnchor).Active = true;
            var bubble5 = _bubbles[4].Subviews[0];
            bubble5.TopAnchor.ConstraintEqualTo(_bubbles[4].TopAnchor, margin).Active = true;
            bubble5.LeadingAnchor.ConstraintEqualTo(_bubbles[4].LeadingAnchor).Active = true;
            var bubble6 = _bubbles[5].Subviews[0];
            bubble6.BottomAnchor.ConstraintEqualTo(_bubbles[5].BottomAnchor, -margin).Active = true;
            bubble6.LeadingAnchor.ConstraintEqualTo(_bubbles[5].LeadingAnchor).Active = true;
        }
    }
}
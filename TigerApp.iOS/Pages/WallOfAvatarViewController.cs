using AD.iOS;
using Foundation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TigerApp.iOS.Utils;
using TigerApp.iOS.Views;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using UIKit;
using System.Reactive.Linq;

namespace TigerApp.iOS.Pages
{
    [Register("WallOfAvatarViewController")]
    public class WallOfAvatarViewController : BaseReactiveViewController<WallOfAvatarViewModel>
    {
        private List<UIButton> _avatarButtons = new List<UIButton>();
        private UIButton _confirmCommand;
        private UIStackView _firstRowStack;
        private UIStackView _secondRowStack;
        private UIStackView _thirdRowStack;
        private ActivityIndicatorView _activityIndicator;


        public WallOfAvatarViewController()
        {
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

            this.WhenActivated((dis) =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Avatars).Where(avatars => avatars != null).Subscribe(avatars =>
                {
                    SetAvatars(avatars);
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.IsBusy).Subscribe(isBusy => { _activityIndicator.Hidden = !isBusy; }));

                dis(ViewModel.WhenAnyValue(vm => vm.UpdateFinished).Subscribe(finished =>
                {
                    if (finished)
                    {
                        if (IsPresentedBy<Profile.ProfileViewController>())
                        {
                            DismissViewController(true, null);
                        }
                        else
                        {
                            var presentingController = PresentingViewController;
                            if (IsPresentedBy < SmsEnrollmentViewController>()) {
                                presentingController = this;
                            }else
                                DismissViewController(false, null);
                            SetRootViewController(ViewModel.Profile);
                        }
                    }
                }));

                dis(this.BindCommand(ViewModel, x => x.UpdateAvatar, x => x._confirmCommand));

                ViewModel.GetUserAvatars();
            });
        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView(UILayoutConstraintAxis.Vertical, UIStackViewDistribution.FillProportionally, alignment: UIStackViewAlignment.Center, spacing: 15);

            var logoImage = UIImage.FromBundle("FlyingTigerLogo");
            nfloat logoImageAspectratio = logoImage.Size.Height / logoImage.Size.Width;

            var logoImageView = new UIImageView();
            logoImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            logoImageView.Image = logoImage;
            logoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var message = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black);
            message.Text = "scegli il tuo avatar";

            _firstRowStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, UIStackViewDistribution.FillEqually, UIStackViewAlignment.Center, 20);
            _firstRowStack.LayoutMargins = new UIEdgeInsets(0, 20, 0, 20);
            _firstRowStack.LayoutMarginsRelativeArrangement = true;

            _secondRowStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, UIStackViewDistribution.FillEqually, UIStackViewAlignment.Center, 20);
            _secondRowStack.LayoutMargins = new UIEdgeInsets(0, 20, 0, 20);
            _secondRowStack.LayoutMarginsRelativeArrangement = true;

            _thirdRowStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, UIStackViewDistribution.FillEqually, UIStackViewAlignment.Center, 20);
            _thirdRowStack.LayoutMargins = new UIEdgeInsets(0, 20, 0, 20);
            _thirdRowStack.LayoutMarginsRelativeArrangement = true;

            _confirmCommand = UICommon.CreateButton("Conferma");
            _confirmCommand.SetBackgroundImage(UIImage.FromBundle("GrayButtonBackground"), UIControlState.Disabled);

            var avtarStack = UICommon.CreateStackView(UILayoutConstraintAxis.Vertical, UIStackViewDistribution.FillEqually, UIStackViewAlignment.Fill, 15);

            _activityIndicator = new ActivityIndicatorView();

            mainStack.AddArrangedSubview(logoImageView);
            mainStack.AddArrangedSubview(message);
            mainStack.AddArrangedSubview(new UIView());
            avtarStack.AddArrangedSubview(_firstRowStack);
            avtarStack.AddArrangedSubview(_secondRowStack);
            avtarStack.AddArrangedSubview(_thirdRowStack);
            mainStack.AddArrangedSubview(avtarStack);
            mainStack.AddArrangedSubview(_confirmCommand);

            View.Add(mainStack);
            View.Add(_activityIndicator);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 0.5f, 0),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Top, 1, 30),
                NSLayoutConstraint.Create(logoImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, logoImageView, NSLayoutAttribute.Width, logoImageAspectratio, 0),

                NSLayoutConstraint.Create(message, NSLayoutAttribute.Top, NSLayoutRelation.Equal, logoImageView, NSLayoutAttribute.Bottom, 1, 20),
                NSLayoutConstraint.Create(message, NSLayoutAttribute.Height, NSLayoutRelation.GreaterThanOrEqual, null, NSLayoutAttribute.NoAttribute, 0, 60),

                NSLayoutConstraint.Create(avtarStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(_confirmCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Width, 1, -20),
                NSLayoutConstraint.Create(_confirmCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, mainStack, NSLayoutAttribute.Bottom, 1, -10),

                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(_activityIndicator, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),
            });
        }

        private void SetAvatars(List<Avatar> avatars)
        {
            avatars = avatars.OrderBy(a => a.Slug).Take(6).ToList();

            avatars.ForEach(async a =>
            {
                var img = await LoadImage(a.ImageUrl);
                nfloat imgAspectRatio = img.Size.Height / img.Size.Width;

                var avatarButton = UIButton.FromType(UIButtonType.Custom);
                avatarButton.TranslatesAutoresizingMaskIntoConstraints = false;
                avatarButton.SetImage(img, UIControlState.Normal);
                avatarButton.SetBackgroundImage(UIImage.FromBundle("avatar_01"), UIControlState.Normal);
                avatarButton.SetBackgroundImage(UIImage.FromBundle("avatar_01_on"), UIControlState.Selected);
                avatarButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                avatarButton.Tag = avatars.IndexOf(a);

                avatarButton.AddConstraints(new NSLayoutConstraint[]
                {
                    NSLayoutConstraint.Create(avatarButton.ImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, avatarButton.ImageView, NSLayoutAttribute.Width, imgAspectRatio, 0),
                    NSLayoutConstraint.Create(avatarButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, avatarButton, NSLayoutAttribute.Width, imgAspectRatio, 0),
                });

                _avatarButtons.Add(avatarButton);

                avatarButton.TouchUpInside += (s, e) =>
                {
                    var selectedButton = s as UIButton;
                    _avatarButtons.Except(new[] { selectedButton }).ToList().ForEach(ab => ab.Selected = false);
                    selectedButton.Selected = !selectedButton.Selected;
                    ViewModel.SelectedAvatarId = selectedButton.Selected ? avatars[(int)selectedButton.Tag].Id : string.Empty;
                };

                if (ViewModel.Profile.Avatar != null)
                {
                    avatarButton.Selected = ViewModel.Profile.Avatar.Id == a.Id;
                    if(avatarButton.Selected)
                        ViewModel.SelectedAvatarId = a.Id;
                }

                switch (avatars.IndexOf(a))
                {
                    case 0:
                    case 1:
                        _firstRowStack.AddArrangedSubview(avatarButton);
                        break;
                    case 2:
                    case 3:
                        _secondRowStack.AddArrangedSubview(avatarButton);
                        break;
                    case 4:
                    case 5:
                        _thirdRowStack.AddArrangedSubview(avatarButton);
                        break;
                }

            });

            _activityIndicator.Hidden = true;
        }

        private async Task<UIImage> LoadImage(string imageUrl)
        {
            var httpClient = new System.Net.Http.HttpClient();
            Task<byte[]> contentsTask = httpClient.GetByteArrayAsync(AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + imageUrl);
            var contents = await contentsTask;
            return UIImage.LoadFromData(NSData.FromArray(contents));
        }
    }
}
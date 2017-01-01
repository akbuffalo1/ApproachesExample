using Foundation;
using ReactiveUI;
using TigerApp.iOS.Tutorial;
using TigerApp.iOS.Utils;
using TigerApp.iOS.Views;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;
using UIKit;
using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;

namespace TigerApp.iOS.Pages
{
    [Register("HomeViewController")]
    public class HomeViewController : BaseReactiveViewController<IHomeViewModel>, IUITextFieldDelegate
    {
        private readonly UIColor COLOR_BUTTON_ENABLED = UIColor.FromRGB(252, 146, 41);
        private readonly UIColor COLOR_BUTTON_DISABLED = UIColor.FromRGB(171, 171, 171);

        [Export("textField:shouldChangeCharactersInRange:replacementString:")]
        public bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
            var oldLength = textField.Text.Length;
            var replacementLength = replacementString.Length;
            var rangeLength = range.Length;
            var newLength = oldLength - rangeLength + replacementLength;
            var returnKey = replacementString.IndexOf('\n') != -1;
            return newLength <= ViewModel.PhoneNumberMaxLength || returnKey;
        }

        private void BottomRightButton_Clicked(object sender, EventArgs e)
        {
            PresentViewController(new StoreLocator.StoreLocatorViewController(), true, null);
        }

        private void BottomLeftButton_Clicked(object sender, EventArgs e)
        {
            PresentViewController(new CatalogueOfProducts.CatalogueOfProductsViewController(), true, null);
        }

        public UIButton _signInRegisterCommand { get; private set; }
        public UITextField _phoneTextField { get; private set; }
        public UIButton _signInWithFacebookCommand { get; private set; }

        public UIBarButtonItem bottomLeftButton { get; private set; }
        public UIBarButtonItem bottomRightButton { get; private set; }

        public HomeViewController(bool showTutorial = false)
        {
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

            var confirmSms = ReactiveCommand.CreateAsyncObservable(Observable.Return(true), (object arg) =>
            {
                return Observable.Create<Unit>(obs =>
                {
                    // TODO: translations
                    var alertWindow = UIAlertController.Create("TITLE", $"Hai inserito il numero {ViewModel.PhoneNumber}, è quello corretto?", UIAlertControllerStyle.Alert);
                    alertWindow.AddAction(UIAlertAction.Create("Confirm", UIAlertActionStyle.Default, (UIAlertAction obj) => { }));
                    alertWindow.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (UIAlertAction obj) => { }));
                    PresentViewController(alertWindow, animated: true, completionHandler: null);
                    return Disposable.Empty;
                }).ObserveOnUI();
            });

            this.WhenActivated(dis =>
            {
                dis(this.BindCommand(ViewModel, vm => vm.PerformFacebookLogin, vc => vc._signInWithFacebookCommand));
                dis(this.BindCommand(ViewModel, vm => vm.PerformSmsLogin, vc => vc._signInRegisterCommand));
                dis(this.WhenAnyValue(x => x._phoneTextField.Text).BindTo(this, x => x.ViewModel.PhoneNumber));

                dis(ViewModel.WhenAnyValue(vm => vm.IsPhoneNumberValid).Subscribe(valid =>
                {
                    if (!valid)
                    {
                        _signInRegisterCommand.TintColor = COLOR_BUTTON_DISABLED;
                    }
                    else {
                        _signInRegisterCommand.TintColor = COLOR_BUTTON_ENABLED;
                    }
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.SmsSent).Where(didSend => didSend == true).Subscribe((obj) =>
                {
                    InvokeOnMainThread(() =>
                    {
                        PresentViewController(new SmsEnrollmentViewController(), true, null);
                    });
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.SignedInWithFacebook).Where(didSend => didSend == true).Subscribe((obj) =>
                {
                    SetRootViewControllerWithUserProfileRequest();
                }));

                if (ViewModel.ShouldShowTutorial)
                {
                    PresentViewController(new HomeTutorialViewController(this), true, null);
                }
            });
        }

        public UIToolbar bottomToolbar { get; private set; }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center);

            var topStripe = new UIView { BackgroundColor = Colors.HexF3F3F2 };
            topStripe.HeightAnchor.ConstraintEqualTo(64).Active = true;

            var topImage = new UIImageView();
            topImage.Image = UIImage.FromBundle("home_tut_08");
            topImage.WidthAnchor.ConstraintEqualTo(185).Active = true;
            topImage.HeightAnchor.ConstraintEqualTo(50).Active = true;

            var welcomeMessage = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), Colors.Hex4D4D4D);
            welcomeMessage.AttributedText = new NSAttributedString(Constants.Strings.HomeWelcomeMessage,
                new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center } });

            var phoneField = new CustomTextField();
            phoneField.SetIcon("home_tut_07");
            phoneField.PlaceholderKey = "Num. telefono";
            phoneField.HideDivider();

            _phoneTextField = phoneField.TextField;

            var divider = UICommon.CreateDivider(1f);

            _signInRegisterCommand = UICommon.CreateButton("Accedi / Registrati");

            _signInRegisterCommand.SetBackgroundImage(UIImage.FromBundle("home_tut_05_button").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            _signInRegisterCommand.TintColor = COLOR_BUTTON_DISABLED;
            _signInRegisterCommand.TouchUpInside += (sender, e) => {
                _signInRegisterCommand.Enabled = false;
            };

            var verificationMessage = UICommon.CreateLabel(Fonts.FrutigerRegular.Pt16, Colors.Hex999999, UITextAlignment.Center);
            verificationMessage.Text = Constants.Strings.HomeVerificationMessage;

            var orMessage = UICommon.CreateLabel(Fonts.FrutigerMedium.Pt20, Colors.Hex4D4D4D, UITextAlignment.Center);
            orMessage.Text = Constants.Strings.HomeOrMessage;

            _signInWithFacebookCommand = UICommon.CreateButton("Accedi con Facebook", "home_tut_06_button");
            _signInWithFacebookCommand.TouchUpInside += (sender, e) =>
            {
                _signInWithFacebookCommand.Enabled = false;
            };

            bottomToolbar = new UIToolbar { TintColor = UIColor.Black, BackgroundColor = UIColor.White, Translucent = false };
            bottomLeftButton = new UIBarButtonItem(UIImage.FromBundle("home_tut_09_button"), UIBarButtonItemStyle.Plain, (s, e) => { });
            bottomRightButton = new UIBarButtonItem(UIImage.FromBundle("home_tut_10_button"), UIBarButtonItemStyle.Plain, (s, e) => { });

            bottomLeftButton.Clicked += BottomLeftButton_Clicked;
            bottomRightButton.Clicked += BottomRightButton_Clicked;

            bottomToolbar.Items = new UIBarButtonItem[]
            {
                bottomLeftButton,
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                bottomRightButton
            };
            bottomToolbar.HeightAnchor.ConstraintEqualTo(44).Active = true;

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(topImage);
            mainStack.AddArrangedSubview(welcomeMessage);
            mainStack.AddArrangedSubview(phoneField);
            mainStack.AddArrangedSubview(divider);
            mainStack.AddArrangedSubview(_signInRegisterCommand);
            mainStack.AddArrangedSubview(verificationMessage);
            mainStack.AddArrangedSubview(orMessage);
            mainStack.AddArrangedSubview(_signInWithFacebookCommand);
            mainStack.AddArrangedSubview(new UIView());
            mainStack.AddArrangedSubview(bottomToolbar);

            View.Add(mainStack);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),
                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(topImage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, -32),

                NSLayoutConstraint.Create(welcomeMessage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topImage, NSLayoutAttribute.Bottom, 1, 35),
                NSLayoutConstraint.Create(welcomeMessage, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 40),
                NSLayoutConstraint.Create(welcomeMessage, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -80),

                NSLayoutConstraint.Create(phoneField, NSLayoutAttribute.Top, NSLayoutRelation.Equal, welcomeMessage, NSLayoutAttribute.Bottom, 1, 25),
                NSLayoutConstraint.Create(phoneField, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -100),

                NSLayoutConstraint.Create(divider, NSLayoutAttribute.Top, NSLayoutRelation.Equal, phoneField, NSLayoutAttribute.Bottom, 1, 10),
                NSLayoutConstraint.Create(divider, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 10),
                NSLayoutConstraint.Create(divider, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -20),

                NSLayoutConstraint.Create(_signInRegisterCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, divider, NSLayoutAttribute.Bottom, 1, 10),
                NSLayoutConstraint.Create(_signInRegisterCommand, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 5),
                NSLayoutConstraint.Create(_signInRegisterCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -10),

                NSLayoutConstraint.Create(verificationMessage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _signInRegisterCommand, NSLayoutAttribute.Bottom, 1, 5),
                NSLayoutConstraint.Create(verificationMessage, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 5),
                NSLayoutConstraint.Create(verificationMessage, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -10),

                NSLayoutConstraint.Create(orMessage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, verificationMessage, NSLayoutAttribute.Bottom, 1, 30),
                NSLayoutConstraint.Create(orMessage, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 40),
                NSLayoutConstraint.Create(orMessage, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -80),

                NSLayoutConstraint.Create(_signInWithFacebookCommand, NSLayoutAttribute.Top, NSLayoutRelation.Equal, orMessage, NSLayoutAttribute.Bottom, 1, 30),
                NSLayoutConstraint.Create(_signInWithFacebookCommand, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 5),
                NSLayoutConstraint.Create(_signInWithFacebookCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -10),

                NSLayoutConstraint.Create(bottomToolbar, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
            });

            //var storeLocator = new StoreLocator.StoreLocatorViewController();
            //var productsCatalogue = new CatalogueOfProducts.CatalogueOfProductsViewController();

            View.AddGestureRecognizer(this.LeftSwipeTo<StoreLocator.StoreLocatorViewController>());
            View.AddGestureRecognizer(this.LeftSwipeTo<CatalogueOfProducts.CatalogueOfProductsViewController>());

            _phoneTextField.EnablesReturnKeyAutomatically = true;
            _phoneTextField.Delegate = this;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            View.EndEditing(true);
        }
    }
}
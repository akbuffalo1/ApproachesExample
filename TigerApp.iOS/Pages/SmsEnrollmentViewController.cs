using TigerApp.Shared.ViewModels;
using System.Reactive.Linq;
using ReactiveUI;
using System;
using UIKit;

namespace TigerApp.iOS.Pages
{
    public partial class SmsEnrollmentViewController : BaseReactiveViewController<ISmsEnrollmentViewModel>
    {
        public SmsEnrollmentViewController()
        {
            this.WhenActivated(dis =>
            {
                dis(this.WhenAnyValue(x => x.verificationCodeCustomTextField.TextField.Text).BindTo(this, x => x.ViewModel.VerificationCode));
                dis(this.BindCommand(ViewModel, vm => vm.PerformSmsVerification, vc => vc.verifyCodeButton));
                dis(ViewModel.WhenAnyValue(vm => vm.SignedInWithSms).Where(didSend => didSend == true).Subscribe((obj) =>
                {
                    InvokeOnMainThread(() =>
                    {
                        SetRootViewControllerWithUserProfileRequest();
                    });
                }));
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            verificationCodeCustomTextField.SetKeyboard(UIKit.UIKeyboardType.Default);
        }

        public override void TouchesBegan(Foundation.NSSet touches, UIKit.UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            View.EndEditing(true);
        }
    }
}
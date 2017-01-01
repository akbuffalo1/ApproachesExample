using Foundation;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.EditProfile
{
    public partial class EditProfileViewController : BaseReactiveViewController<IEditProfileViewModel>
    {
        private TigerDatePicker compleanoDateField => compleanoField as TigerDatePicker;
        private TigerCityPicker tigerCityField => cityField as TigerCityPicker;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupConfirmButton("Conferma dati\nper completare la missione");
            SetupTextFields();
        }

        private void SetupTextFields()
        {
            nomeField.Font = Fonts.FrutigerMedium.WithSize(19);
            emailField.Font = Fonts.FrutigerMedium.WithSize(19);
            cognomeField.Font = Fonts.FrutigerMedium.WithSize(19);
            nicknameField.Font = Fonts.FrutigerMedium.WithSize(19);
            compleanoField.Font = Fonts.FrutigerMedium.WithSize(19);
            telephoneField.Font = Fonts.FrutigerMedium.WithSize(19);
            cityField.Font = Fonts.FrutigerMedium.WithSize(19);
        }

        partial void OnBackButtonClick(NSObject sender)
        {
            DismissViewController(true, null);
        }

        private void SetupConfirmButton(string buttonTitle)
        {
            var buttonTitleParts = buttonTitle.Split('\n');
            string buttonTitleAttrText;
            string title;
            string subtitle;

            if (buttonTitleParts.Length == 2)
            {
                title = buttonTitleParts[0];
                subtitle = buttonTitleParts[1];
                buttonTitleAttrText = $"{title}\n{subtitle}";
            }
            else
            {
                buttonTitleAttrText = buttonTitle;
                title = buttonTitle;
                subtitle = string.Empty;
            }

            var attrText = new NSMutableAttributedString(buttonTitleAttrText);

            var titleRange = new NSRange(0, title.Length);
            var subtitleRange = new NSRange(title.Length + 1, subtitle.Length);
            var allRange = new NSRange(0, attrText.Value.Length);

            var paragraphStyle = new NSMutableParagraphStyle();
            paragraphStyle.Alignment = UITextAlignment.Center;

            paragraphStyle.LineSpacing = 0.15F;
            paragraphStyle.MinimumLineHeight = 25;
            paragraphStyle.MaximumLineHeight = 25;

            attrText.AddAttribute(UIStringAttributeKey.ParagraphStyle, paragraphStyle, allRange);
            attrText.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, allRange);

            attrText.AddAttribute(UIStringAttributeKey.Font, Fonts.TigerBasic.WithSize(30F), titleRange);
            attrText.AddAttribute(UIStringAttributeKey.Font, Fonts.TigerBasic.WithSize(26F), subtitleRange);

            confirmButton.TitleLabel.Lines = 0;
            confirmButton.TitleLabel.AttributedText = attrText;
            confirmButton.TitleLabel.SetNeedsLayout();
            confirmButton.TitleLabel.SetNeedsDisplay();
            confirmButton.SetAttributedTitle(attrText, UIControlState.Normal);
            confirmButton.Enabled = false;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            View.EndEditing(true);
        }

        public EditProfileViewController()
        {
            TransitioningDelegate = TransitionManager.Right;

            this.WhenActivated(dis =>
            {
                dis(this.BindCommand(ViewModel, vm => vm.UpdateProfile, vc => vc.confirmButton));

                dis(this.Bind(ViewModel, vm => vm.Nickname, vc => vc.nicknameField.Text));
                dis(this.Bind(ViewModel, vm => vm.LastName, vc => vc.cognomeField.Text));
                dis(this.Bind(ViewModel, vm => vm.Email, vc => vc.emailField.Text));
                dis(this.Bind(ViewModel, vm => vm.FirstName, vc => vc.nomeField.Text));
                dis(this.Bind(ViewModel, vm => vm.MobileNumber, vc => vc.telephoneField.Text));

                dis(ViewModel.WhenAnyValue(vm => vm.BirthdayDate).Subscribe(birthdayDate =>
                {
                    compleanoDateField.SetDateTime(birthdayDate);
                }));

                dis(this.WhenAnyObservable(vc => vc.compleanoDateField.Date).Subscribe(dateText =>
                {
                    ViewModel.BirthdayDate = dateText;
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.TigerCity).Subscribe(city => { tigerCityField.SetCity(city); }));
                dis(ViewModel.WhenAnyValue(vm => vm.TigerStoreCities).Subscribe(cities => { tigerCityField.SetCities(cities); }));
                dis(this.WhenAnyObservable(vc => vc.tigerCityField.City).Subscribe(city => { ViewModel.TigerCity = city; }));

                dis(ViewModel.WhenAnyValue(vm => vm.MissionAlreadyCompleted).Where(_ => _ == true).Subscribe(missionCompleted =>
                {
                    SetupConfirmButton("Conferma");
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.UpdateFinished).Where(_ => _ == true).Subscribe(_ =>
                {
                    DismissViewController(true, null);
                    /*if(!ViewModel.MissionAlreadyCompleted)
                        PresentViewController(new EditProfileConfirmationPopupViewController(this), true, null);*/
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.IsProfileComplete).Subscribe(isProfileEditingCompleted =>
                {
                    if (isProfileEditingCompleted)
                    {
                        confirmButton.Enabled = true;
                    }
                    else
                    {
                        confirmButton.Enabled = false;
                    }
                }));
            });
        }
    }
}
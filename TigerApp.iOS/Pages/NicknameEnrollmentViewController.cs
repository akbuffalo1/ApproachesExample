using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Foundation;
using ReactiveUI;
using TigerApp.iOS;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    public partial class NicknameEnrollmentViewController : BaseReactiveViewController<INicknameEnrollmentViewModel>
    {
        public NicknameEnrollmentViewController()
        {
            this.WhenActivated(dis =>
            {
                dis(this.WhenAnyValue(x => x.txtFieldNickname.TextField.Text).BindTo(ViewModel, x => x.UserNickname));
                dis(this.BindCommand(ViewModel, vm => vm.UpdateNickname, vc => vc.registerButton));
                dis(ViewModel.WhenAnyValue(vm => vm.AvatarImageUrl).Where(url => !string.IsNullOrEmpty(url)).Subscribe(avatarUrl => {
                    SetAvatarImage(avatarUrl);
                }));
                dis(ViewModel.WhenAnyValue(vm => vm.NicknameUpdated).Where(_ => _ == true).Subscribe(nicknameUpdated =>
                {
                    PresentViewController(new AcceptEnrollmentConditionsViewController(), true, null);
                }));
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            txtFieldNickname.SetKeyboard(UIKeyboardType.NamePhonePad);
        }

        private async void SetAvatarImage(string avatarUrl) {
            var backgroundImage = UIImage.FromBundle("enroll_placeholder").Scale(avatarImage.Frame.Size);
            avatarImage.BackgroundColor = UIColor.FromPatternImage(backgroundImage);
            avatarImage.Image = await LoadImage(avatarUrl);
        }

        private async Task<UIImage> LoadImage(string imageUrl)
        {
            var httpClient = new System.Net.Http.HttpClient();
            Task<byte[]> contentsTask = httpClient.GetByteArrayAsync(imageUrl);
            var contents = await contentsTask;
            return UIImage.LoadFromData(NSData.FromArray(contents));
        }
    }
}
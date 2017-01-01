using System;
using System.Reactive.Linq;
using AD;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ReactiveUI;
using TigerApp.Droid.Utils;
using TigerApp.Shared.ViewModels;
using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/facebook_app_id")]
namespace TigerApp.Droid.Pages
{
    [Activity]
    public class NicknameEnrollmentActivity : BaseReactiveActivity<INicknameEnrollmentViewModel>
    {
        private Button btnRegister { get; set; }
        private EditText txtNicknameInput { get; set; }
        private AD.Views.Android.ADImageView avatarImageView;

        public NicknameEnrollmentActivity()
        {
            this.WhenActivated(dispose =>
            {
                dispose(this.WhenAnyValue(v => v.txtNicknameInput.Text).BindTo(ViewModel, vm => vm.UserNickname));
                dispose(ViewModel.WhenAnyValue(vm => vm.AvatarImageUrl).Where(url => !string.IsNullOrEmpty(url)).BindTo(this, v => v.avatarImageView.ImageUrl));
                dispose(this.BindCommand(ViewModel, vm => vm.UpdateNickname, v => v.btnRegister));
                dispose(ViewModel.WhenAnyValue(vm => vm.UserNickname).Select(name => !string.IsNullOrEmpty(name)).BindTo(this, v => v.btnRegister.Enabled));
                dispose(ViewModel.WhenAnyValue(vm => vm.NicknameUpdated).Where(_ => _ == true).Subscribe(nicknameUpdated =>
                {
                    DialogUtil.ShowAlert(this, this.Resources.GetString(Resource.String.privacy_accept_message), this.Resources.GetString(Resource.String.txt_accept),
                                         () => { 
                        StartNewActivity(typeof(ExpHomeActivity), TransitionWay.RL);
                                             Finish();
                    });
                }));
            });
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_nickname_enrollment);

            btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            txtNicknameInput = FindViewById<EditText>(Resource.Id.txtNicknameInput);
            avatarImageView = FindViewById<AD.Views.Android.ADImageView>(Resource.Id.avatarImageView);
        }
    }
}
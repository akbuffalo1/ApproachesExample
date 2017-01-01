using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AD;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TigerApp.Shared.Models.Requests;
using TigerApp.Shared.Models.Responses;
using TigerApp.Shared.Utils;

namespace TigerApp.Shared.ViewModels
{
    public interface IHomeViewModel : IViewModelBase
    {
        bool SmsSent { get; }
        bool SignedInWithFacebook { get; }
        bool IsPhoneNumberValid { get; }
        IReactiveCommand PerformFacebookLogin { get; }
        IReactiveCommand PerformSmsLogin { get; }
        string PhoneNumber { get; set; }
        bool ShouldShowTutorial { get; }
        int PhoneNumberMaxLength { get; }
    }

    public class HomeViewModel : ReactiveViewModel, IHomeViewModel
    {
        public int PhoneNumberMaxLength => 10;

        public IReactiveCommand PerformFacebookLogin
        {
            get;
            protected set;
        }

        public IReactiveCommand PerformSmsLogin
        {
            get;
            protected set;
        }

        [Reactive]
        public bool SmsSent
        {
            get;
            protected set;
        }

        public bool ShouldShowTutorial => !FlagStore.IsSet(Constants.Flags.HOME_TUTORIAL_SHOWN);

        [Reactive]
        public string PhoneNumber
        {
            get;
            set;
        }

        [Reactive]
        public bool SignedInWithFacebook
        {
            get;
            protected set;
        }

        public extern bool IsPhoneNumberValid
        {
            [ObservableAsProperty]
            get;
        }

        private readonly IFacebookAuthService _fbAuthService;
        private readonly IAuthApiService _authService;
        private readonly ITDesAuthStore _tigerToken;

        public HomeViewModel(ITDesAuthStore tigerToken, IAuthApiService authApi, IFacebookAuthService fbAuthService)
        {
            _tigerToken = tigerToken;
            _fbAuthService = fbAuthService;
            _authService = authApi;

            _fbAuthService.SignIn.Subscribe((obj) =>
            {
                if(obj != null)
                    OnFacebookAuth(true);
            });

            PerformFacebookLogin = _fbAuthService.SignIn;

            var isPhoneNumberValidObservable = this.WhenAnyValue(x => x.PhoneNumber, (arg) => arg?.Length == PhoneNumberMaxLength);
            isPhoneNumberValidObservable.ToPropertyEx(this, x => x.IsPhoneNumberValid);

            PerformSmsLogin = ReactiveCommand.CreateAsyncObservable(isPhoneNumberValidObservable, (arg) =>
            {
                return Observable.Start(PerformSmsLoginRequest);
            });
        }

        private void PerformSmsLoginRequest()
        {
            var internationalPhoneNumber = $"+39{PhoneNumber}";

            _authService.SmsLogin(new SmsLoginRequest
            {
                phone_number = internationalPhoneNumber,
            })
            .SubscribeOnce((SmsLoginResponse data) =>
            {
                _authService.SmsLoginPhoneNumber = internationalPhoneNumber;
                SmsSent = true;
            });
        }

        private void OnFacebookAuth(bool signedIn)
        {
            if (!signedIn)
            {
                UserError.Throw(new UserError("To continue, sign in with FB"));
                return;
            }

            _authService.FacebookLogin(new FacebookLoginRequest
            {
                token = _fbAuthService.Token,
                user_id = _fbAuthService.UserId
            })
            .SubscribeOnce((FacebookLoginResponse data) =>
            {
                var authData = _tigerToken.GetAuthData();
                if (data?.Token != null)
                {
                    authData.AuthProviderToken = data?.Token;
                    authData.AuthProvider = "facebook";
                    _tigerToken.SetAuthData(authData);
                }
                SignedInWithFacebook = true;
                AD.Resolver.Resolve<Services.API.IProfileApiService>().UpdateUserLoginStatus(true);
            });
        }
    }
}

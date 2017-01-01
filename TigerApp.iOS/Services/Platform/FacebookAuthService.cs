using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;

using Facebook.CoreKit;
using Facebook.LoginKit;

using TigerApp.Shared.Config;
using Foundation;
using ReactiveUI;
using System.Reactive.Threading.Tasks;

using TigerApp.Shared.Models.Responses;
using System.Reactive.Disposables;

namespace TigerApp.iOS.Services.Platform
{
    public class FacebookAuthService : IFacebookAuthService
    {
        public IObservable<bool> IsSignedIn => _isSignedIn;

        private readonly LoginManager _fbLoginManager;
        private readonly IFacebookAuthConfig _fbAuthConfig;
        private readonly ISubject<bool> _isSignedIn;

        private readonly IObservable<bool> _canSignIn;
        private readonly IObservable<bool> _canSignOut;

        private bool HasValidToken => !string.IsNullOrEmpty(AccessToken.CurrentAccessToken?.TokenString);

        public IReactiveCommand<FacebookAuthResponse> SignIn
        {
            get;
            protected set;
        }

        public IReactiveCommand<FacebookAuthResponse> SignOut
        {
            get;
            protected set;
        }

        public string Token => AccessToken.CurrentAccessToken.TokenString;
        public string UserId => AccessToken.CurrentAccessToken.UserID;

        protected void OnProfileChanged(object sender, ProfileDidChangeEventArgs e)
        {
            if (e.NewProfile == null)
            {
                _isSignedIn.OnNext(false);
            }
            else
            {
                _isSignedIn.OnNext(true);
            }
        }

        public FacebookAuthService(IFacebookAuthConfig fbAuthConfig)
        {
            _fbAuthConfig = fbAuthConfig;
            _isSignedIn = new BehaviorSubject<bool>(HasValidToken);

            _canSignOut = Observable.Return(true); //IsSignedIn.Select(x => x == true);
            _canSignIn = Observable.Return(true); // IsSignedIn.Select(x => x == false);

            Settings.AppID = _fbAuthConfig.AppId;
            Settings.DisplayName = _fbAuthConfig.AppName;

            Profile.EnableUpdatesOnAccessTokenChange(true);
            Profile.Notifications.ObserveDidChange(OnProfileChanged);

            _fbLoginManager = new LoginManager();
            _fbLoginManager.LoginBehavior = LoginBehavior.SystemAccount;

            //var fbObservable = Observable.Defer(() => _fbLoginManager.LogInWithReadPermissionsAsync(_fbAuthConfig.ReadPermissions, null).ToObservable());

            SignIn = ReactiveCommand.CreateAsyncObservable(_canSignIn, o =>
            Observable.Create<FacebookAuthResponse>(async obs =>
            {
                LoginManagerLoginResult loginResult = null;

                try
                {
                    loginResult = await _fbLoginManager.LogInWithReadPermissionsAsync(_fbAuthConfig.ReadPermissions, null);

                    //return fbObservable.Subscribe((LoginManagerLoginResult loginResult) =>
                    //{

                    if (loginResult.IsCancelled)
                    {
                        obs.OnNext(null);
                        obs.OnCompleted();
                    }
                    else
                    {
                        Profile.LoadCurrentProfile((profile, error) =>
                        {
                            obs.OnNext(new FacebookAuthResponse
                            {
                                Token = loginResult.Token.TokenString,
                                DisplayName = profile.Name
                            });
                            obs.OnCompleted();
                        });
                    }
                    //}, (ex) =>
                    //{
                }
                catch (Exception ex)
                {
                    if (ex is NSErrorException)
                        UserError.Throw(((NSErrorException)ex).Error.LocalizedDescription);
                    else
                        UserError.Throw(ex.Message);
                }

                return Disposable.Empty;
                //}, obs.OnCompleted);
            }));

            SignOut = ReactiveCommand.CreateAsyncObservable(_canSignOut, o =>
            {
                _fbLoginManager.LogOut();
                return Observable.Return<FacebookAuthResponse>(null);
            });
        }
    }
}
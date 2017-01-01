using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;

using ReactiveUI;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Android.Runtime;

using TigerApp.Shared.Models.Responses;
using TigerApp.Shared.Config;

using AD.Plugins.CurrentActivity;

namespace TigerApp.Droid.Services.Platform
{
    class CustomProfileTracker : ProfileTracker
    {
        public delegate void CurrentProfileChangedDelegate(Profile oldProfile, Profile currentProfile);

        public CurrentProfileChangedDelegate HandleCurrentProfileChanged { get; set; }

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            var p = HandleCurrentProfileChanged;
            if (p != null)
                p(oldProfile, currentProfile);
        }
    }

    class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
    {
        public Action HandleCancel { get; set; }
        public Action<FacebookException> HandleError { get; set; }
        public Action<TResult> HandleSuccess { get; set; }

        public void OnCancel()
        {
            var c = HandleCancel;
            if (c != null)
                c();
        }

        public void OnError(FacebookException error)
        {
            var c = HandleError;
            if (c != null)
                c(error);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var c = HandleSuccess;
            if (c != null)
                c(result.JavaCast<TResult>());
        }
    }

    public class FacebookAuthService : IFacebookAuthService
    {
        public IObservable<Exception> ThrownExceptions
        {
            get;
            protected set;
        }

        public IObservable<bool> IsSignedIn => _isSignedIn.ObserveOn(RxApp.MainThreadScheduler);
        public string Token => AccessToken.CurrentAccessToken.Token;
        public string UserId => AccessToken.CurrentAccessToken.UserId;

        private readonly LoginManager _fbLoginManager;
        private readonly IFacebookAuthConfig _fbAuthConfig;
        private readonly ISubject<bool> _isSignedIn;

        private readonly IObservable<bool> _canSignIn;
        private readonly IObservable<bool> _canSignOut;

        private bool HasValidToken => !string.IsNullOrEmpty(AccessToken.CurrentAccessToken?.Token);

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

        public FacebookAuthService(IFacebookAuthConfig fbAuthConfig, Func<ICurrentActivity> currentActivity, ICallbackManager callbackManager)
        {
            _fbAuthConfig = fbAuthConfig;

            _isSignedIn = new BehaviorSubject<bool>(HasValidToken);
            _fbLoginManager = LoginManager.Instance;

            _canSignOut = Observable.Return(true); // IsSignedIn.Select(x => x == true);
            _canSignIn = Observable.Return(true); // IsSignedIn.Select(x => x == false);

            var loginSubject = new BehaviorSubject<FacebookAuthResponse>(null);
            loginSubject.SubscribeOnce((ob) => { 
            });

            var profileTracker = new CustomProfileTracker
            {
                HandleCurrentProfileChanged = (oldProfile, currentProfile) =>
                {
                    if (currentProfile == null)
                    {
                        loginSubject.OnNext(null);
                        loginSubject.OnCompleted();
                        _isSignedIn.OnNext(false);
                    }
                    else
                    {
                        loginSubject.OnNext(new FacebookAuthResponse
                        {
                            Token = AccessToken.CurrentAccessToken.Token,
                            DisplayName = currentProfile.Name
                        });
                        loginSubject.OnCompleted();
                        _isSignedIn.OnNext(true);
                    }
                }
            };

            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleCancel = () =>
                {
                    loginSubject.OnNext(null);
                    loginSubject.OnCompleted();
                    _isSignedIn.OnNext(false);
                },
                HandleError = loginError =>
                {
                    loginSubject.OnNext(null);
                    loginSubject.OnCompleted();
                    _isSignedIn.OnNext(false);
                },
                HandleSuccess = (LoginResult result) => {
                    if (Profile.CurrentProfile?.Name != null)
                    {
                        loginSubject.OnNext(new FacebookAuthResponse
                        {
                            Token = AccessToken.CurrentAccessToken.Token,
                            DisplayName = Profile.CurrentProfile?.Name
                        });
                        loginSubject.OnCompleted();
                        //_isSignedIn.OnNext(true);
                    }
                }
            };

            _fbLoginManager.SetLoginBehavior(LoginBehavior.NativeWithFallback);
            _fbLoginManager.RegisterCallback(callbackManager, loginCallback);

            SignIn = ReactiveCommand.CreateAsyncObservable(_canSignIn, o =>
            {
                var curActivity = currentActivity().Activity;
                _fbLoginManager.LogInWithReadPermissions(curActivity, _fbAuthConfig.ReadPermissions);
                return loginSubject;
            });

            SignOut = ReactiveCommand.CreateAsyncObservable(_canSignOut, o =>
            {
                _fbLoginManager.LogOut();
                return loginSubject;
            });

            // Merge error observables
            ThrownExceptions = SignIn.ThrownExceptions.Merge(SignOut.ThrownExceptions);
        }
    }
}


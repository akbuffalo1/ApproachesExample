using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AD;
using AD.Plugins.OAuth;
using Newtonsoft.Json.Linq;
using ReactiveUI.Fody.Helpers;
using Xamarin.Auth;

namespace TigerApp.Shared.Services.API
{
	public interface ITwitterService
    {
		Account CurrentAccount { get; }
        BehaviorSubject<OAuth1Authenticator> ServiceReadySub { get; }
		void Init();
		void Authorize();
		void Logout();
		IObservable<JObject> GetUserData();
    }

    public class TwitterService : ITwitterService
    {
        const string PROVIDER_NAME = "Twitter";

		[Reactive]
        public Account CurrentAccount { get; protected set; }

		public BehaviorSubject<OAuth1Authenticator> ServiceReadySub { get; protected set; }

		private readonly IOAuthServiceProvider _authProvider;
        private readonly IOAuthAccountHelper _accountHelper;
		public TwitterService(IOAuthServiceProvider authProvider = null, IOAuthAccountHelper accountHelper = null)
        {
            _authProvider = authProvider ?? Resolver.Resolve<IOAuthServiceProvider>();
            _accountHelper = accountHelper ?? Resolver.Resolve<IOAuthAccountHelper>();
            ServiceReadySub = new BehaviorSubject<OAuth1Authenticator>(null);

			_accountHelper
				.FindAccountsObservable(PROVIDER_NAME)
				.Select(accounts => accounts.FirstOrDefault())
          		.Where(account => null != account)
				.Subscribe(account => CurrentAccount = account);

			_authProvider.ConfigLoadedSubject.Subscribe(IsConfigLoaded =>
            {
                var authenticator = _authProvider.ProvideAuthenticator();
                ServiceReadySub.OnNext(authenticator);
                EventHandler<AuthenticatorCompletedEventArgs> accountHandler = null;
                accountHandler = (sender, args) =>
                {
                    _accountHelper.TrySaveIfNotNull(args.Account, PROVIDER_NAME);
                    CurrentAccount = args.Account;
                    authenticator.Completed -= accountHandler;
                };
                authenticator.Completed += accountHandler;
            });
		}

        public void Init() => _authProvider.Init(PROVIDER_NAME.ToLower());

		public void Authorize() => _accountHelper.StartAuthorization(_authProvider.ProvideAuthenticator());

		public void Logout()
		{
			_accountHelper.TryDeleteIfNotNullAndExist(CurrentAccount, PROVIDER_NAME);
			CurrentAccount = null;
		}

        public IObservable<JObject> GetUserData()
        {
            return Observable.Create<JObject>(async obs =>
            {
                try
                {
					var result = await _authProvider.ProvideAccountRequest(CurrentAccount).GetResponseAsync();
                    obs.OnNext(JObject.Parse(result.GetResponseText()));
                    obs.OnCompleted();
                }
                catch (Exception ex)
                {
                    obs.OnError(ex);
                }
                return Disposable.Create(() => Console.WriteLine("return Observable.Create<JObject>(async obs =>"));
            });
        }
	}

    public class AuthEventArgs : EventArgs
    {
        public OAuth1Authenticator Authenticator { get; private set; }
        public AuthEventArgs(OAuth1Authenticator authenticator)
        {
            Authenticator = authenticator;
        }
    }
}

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AD;
using AD.Plugins.OAuth;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace TigerApp.Shared.Services.API
{
	public interface ITwitterService
    {
		BehaviorSubject<Account> CurrentAccount { get; }
        BehaviorSubject<OAuth1Authenticator> ServiceReadySub { get; }
		void Init();
		void Authorize();
		Task Logout();
		IObservable<JObject> GetUserData();
    }

    public class TwitterService : ITwitterService
    {
        const string PROVIDER_NAME = "Twitter";

		public BehaviorSubject<Account> CurrentAccount { get; protected set; }

		public BehaviorSubject<OAuth1Authenticator> ServiceReadySub { get; protected set; }

		private readonly IOAuthServiceProvider _authProvider;
        private readonly IOAuthAccountHelper _accountHelper;
		public TwitterService(IOAuthServiceProvider authProvider = null, IOAuthAccountHelper accountHelper = null)
        {
            _authProvider = authProvider ?? Resolver.Resolve<IOAuthServiceProvider>();
            _accountHelper = accountHelper ?? Resolver.Resolve<IOAuthAccountHelper>();
            ServiceReadySub = new BehaviorSubject<OAuth1Authenticator>(null);
			CurrentAccount = new BehaviorSubject<Account>(null);

			_accountHelper
				.FindAccountsObservable(PROVIDER_NAME)
				.Select(accounts => accounts.FirstOrDefault())
          		.Where(account => null != account)
				.Subscribe(account => CurrentAccount.OnNext(account));

			_authProvider.ConfigLoadedSubject.Subscribe(IsConfigLoaded =>
            {
                var authenticator = _authProvider.ProvideAuthenticator();
                ServiceReadySub.OnNext(authenticator);
                EventHandler<AuthenticatorCompletedEventArgs> accountHandler = null;
                accountHandler = (sender, args) =>
                {
                    _accountHelper.TrySaveIfNotNull(args.Account, PROVIDER_NAME);
                    CurrentAccount.OnNext(args.Account);
                    authenticator.Completed -= accountHandler;
                };
                authenticator.Completed += accountHandler;
            });
		}

        public void Init() => _authProvider.Init(PROVIDER_NAME.ToLower());

		public void Authorize() => _accountHelper.StartAuthorization(_authProvider.ProvideAuthenticator());

		public async Task Logout()
		{
			var account = await CurrentAccount.FirstOrDefaultAsync();
			if (null != account)
			{
				_accountHelper.TryDeleteIfNotNullAndExist(account, PROVIDER_NAME);
				CurrentAccount.OnNext(null);
			}
		}

        public IObservable<JObject> GetUserData()
        {
            return Observable.Create<JObject>(async obs =>
            {
                try
                {
					var account = await CurrentAccount.FirstAsync();
					var result = await _authProvider.ProvideAccountRequest(account).GetResponseAsync();
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

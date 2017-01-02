using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AD;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Services.API;

namespace TigerApp.Shared.ViewModels
{
    public interface ITwitterTestViewModel : IViewModelBase
    {
        IReactiveCommand GetProfileCommand { get; }
		IReactiveCommand LogInCommand { get; }
		IReactiveCommand LogOutCommand { get; }
        bool IsLoaderShowing { get; }
        JObject UserData { get; }
        string Error { get; }
        void CheckInitialized();
        bool IsAuthorized { get; }
    }

    public class TwitterTestViewModel : ReactiveViewModel, ITwitterTestViewModel
    {
        public IReactiveCommand GetProfileCommand { get; protected set; }

		public IReactiveCommand LogInCommand { get; protected set; }

		public IReactiveCommand LogOutCommand { get; protected set; }

        public ISubject<bool> ApiAuthorizedSubject { get; protected set; }

		[Reactive]
		public bool IsLoaderShowing { get; protected set; } = false;

        [Reactive] 
        public JObject UserData { get; set; }

        [Reactive] 
        public string Error { get; set; }

		[Reactive] 
        public bool IsAuthorized { get; set; }

		private readonly ITwitterService _service;
        private readonly INetworkReachability _networkReachabilityService;
        private readonly IAccountSenderService _accountSender;
        public TwitterTestViewModel(
            ITwitterService service = null, 
            INetworkReachability networkReachabilityService = null, 
            IAccountSenderService accountSender = null)
        {
            _service = service ?? Resolver.Resolve<ITwitterService>();
            _networkReachabilityService = networkReachabilityService ?? Resolver.Resolve<INetworkReachability>();
            _accountSender = accountSender ?? Resolver.Resolve<IAccountSenderService>();

            this.WhenActivated(toDispose =>
            {
                var accountObservable = _service.WhenAnyValue(serv => serv.CurrentAccount);
                var canLogoutOrGetProfile = accountObservable.Select(account => null != account);
				var canAuthorize = 
                    _service
					    .ServiceReadySub
					    .AsObservable()
					    .Select(authenticator => null != authenticator)
                        .CombineLatest(canLogoutOrGetProfile, (arg1, arg2) =>
                        {
                            return arg1 && !arg2;});

                accountObservable.SubscribeOnce(account =>
                {
                    if (account == null)
                    {
                        _accountSender
                            .Logout()
                            .Catch(ex =>
                            {

                            })
                            .SubscribeOnce(appAccount =>
                            {

                            });
                    }
                    else 
                    {
                        _accountSender
                            .LoginByTwitterAccount(account)
                            .Catch(ex =>
                            {

                            })
                            .SubscribeOnce(appAccount =>
                            {

                            });
                    }
                });

				LogInCommand = ReactiveCommand.CreateAsyncObservable(canAuthorize, args => Observable.Start(() => _service.Authorize()));
				LogOutCommand = ReactiveCommand.CreateAsyncObservable(canLogoutOrGetProfile, args => Observable.Start(() => _service.Logout()));
                GetProfileCommand = ReactiveCommand.CreateAsyncObservable(canLogoutOrGetProfile, args => Observable.Start(() =>
                {
					IsLoaderShowing = true;
                    _service
						.GetUserData()
						.ObserveOnUI()
                    	.Catch(ex =>
	                    {
	                        IsLoaderShowing = false;
	                        Error = ex.Message;
	                    }).SubscribeOnce(data =>
	                    {
	                        UserData = data;
	                        IsLoaderShowing = false;
	                    });
				}));
            });
        }

        public void CheckInitialized() => _service.Init();
    }
}

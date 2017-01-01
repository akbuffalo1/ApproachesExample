using System;
using System.Runtime.CompilerServices;
using AD.Plugins.Network.Rest;
using Xamarin.Auth;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("social")]
    public interface IAccountSenderService : IBaseApiService
    {
        [ApiResourcePath("login/twitter")]
        IObservable<SocialAccount> LoginByTwitterAccount(Account socialAccount);
        [ApiResourcePath("logout")]
        IObservable<object> Logout();
    }

    public class AccountSenderService : ApiServiceProvider, IAccountSenderService
    {
        public IObservable<SocialAccount> LoginByTwitterAccount(Account socialAccount)
        {
            return this.CreateObservableRequestWithoutCatch<SocialAccount, Account>(socialAccount, verb: Verbs.Post);  
        }

        public IObservable<object> Logout()
        {
            return this.CreateObservableRequestWithoutCatch<object, object>(new object(), verb: Verbs.Post);
        }
    }

    public class SocialAccount
    { 
        
    }
}

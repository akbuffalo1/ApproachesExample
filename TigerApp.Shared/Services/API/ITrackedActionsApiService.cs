using System;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models.TrackedActions;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("action")]
    public interface ITrackedActionsApiService : IBaseApiService
    {
        [ApiResourcePath("push/")]
        IObservable<object> PushAction(TrackedAction trackedAction, Action onUnauthorized);
    }

    public class TrackedActionsApiService : ApiServiceProvider, ITrackedActionsApiService
    {
        public IObservable<object> PushAction(TrackedAction trackedAction, Action onUnauthorized) { 
            return this.CreateObservableRequest<object, TrackedAction>(trackedAction, (obs, ex) =>
                {
                    var isUnauthorized = false;

                    if (ex is BetterHttpResponseException)
                    {
                        isUnauthorized = ((BetterHttpResponseException)ex).StatusCode == System.Net.HttpStatusCode.Unauthorized || ((BetterHttpResponseException)ex).StatusCode == System.Net.HttpStatusCode.Forbidden;
                    }

                    if (!isUnauthorized)
                    {
                        throw ex;
                    }
                    else {
                        AD.Resolver.Resolve<IProfileApiService>().UpdateUserLoginStatus(false);
                        onUnauthorized?.Invoke();
                    }
                });
        }
    }
}

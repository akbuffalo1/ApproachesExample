using System;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models.Proximity;

namespace TigerApp.Shared.Services.API
{
    
    public interface IProximityApiService : IBaseApiService
    {
        [ApiResourcePath("proximityevent")]
        IObservable<object> PushProximityEvent(ProximityEvent proximityEvent);
    }

    public class ProximityApiService : ApiServiceProvider, IProximityApiService
    {
        public IObservable<object> PushProximityEvent(ProximityEvent proximityEvent) =>
        this.CreateObservableRequest<object, ProximityEvent>(
            proximityEvent,
            verb: Verbs.Post);
        /*this.CreateFileCacheableObservableRequest<object, ProximityEvent>(proximityEvent, Priority.Internet, "events.dat",
                (porxEv, ex) =>
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
                    }
                },
                verb: Verbs.Post);*/
    }
}

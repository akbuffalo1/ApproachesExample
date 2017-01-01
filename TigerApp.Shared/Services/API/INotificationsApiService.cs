using System;
using System.Reactive.Linq;
using AD.Plugins.Network.Rest;
using TigerApp;
using TigerApp.Shared.Models.Requests;
using TigerApp.Shared.Models.Responses;
using TigerApp.Shared.Services.Platform;

namespace TigerApp.Shared.Services.API
{
    public interface INotificationsApiService : IBaseApiService
    {
        [ApiResourcePath("device")]
        IObservable<RegisterForPushNotificationResponseEntity> SetNotificationsEnabled(System.Collections.Generic.List<Models.ServerAction> request);
    }

    public class NotificationsApiService : ApiServiceProvider, INotificationsApiService
    {
        public IObservable<RegisterForPushNotificationResponseEntity> SetNotificationsEnabled(System.Collections.Generic.List<Models.ServerAction> request) =>
        this.CreateObservableRequestWithoutCatch<RegisterForPushNotificationResponseEntity, System.Collections.Generic.List<Models.ServerAction>>(request,verb:Verbs.Patch);
    }
}


using System;
using System.Collections.Generic;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.Services.API
{
    public interface IAvatarsApiService
    {
        [ApiResourcePath("avatar")]
        IObservable<List<Avatar>> GetAvatars(Priority priority = Priority.Cache);
    }

    public class AvatarsApiService : ApiServiceProvider, IAvatarsApiService
    {
        public IObservable<List<Avatar>> GetAvatars(Priority priority = Priority.Cache) =>
            this.CreateFileCacheableObservableRequest<List<Avatar>, object>(new object(), priority, "avatars.dat",
                (avatars, ex) =>
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
                verb: Verbs.Get);
    }
}

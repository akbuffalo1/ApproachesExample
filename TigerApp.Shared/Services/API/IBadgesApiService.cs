using System;
using System.Collections.Generic;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("badges")]
    public interface IBadgesApiService : IBaseApiService
    {
        [ApiResourcePath("")]
        IObservable<List<Badge>> GetBadges(Priority priority = Priority.Cache);
    }

    public class BadgeApiService : ApiServiceProvider, IBadgesApiService
    {
        public IObservable<List<Badge>> GetBadges(Priority priority = Priority.Cache) =>
        this.CreateFileCacheableObservableRequest<List<Badge>, object>(new object(), priority, "badges.dat",
                (objectuves, ex) =>
                {
                    var isUnauthorized = false;

                    var betterEx = ex as BetterHttpResponseException;
                    if (betterEx != null)
                    {
                        isUnauthorized = betterEx.StatusCode == System.Net.HttpStatusCode.Unauthorized || betterEx.StatusCode == System.Net.HttpStatusCode.Forbidden;
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

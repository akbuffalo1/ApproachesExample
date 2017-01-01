using AD.Plugins.Network.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("mission")]
    public interface IMissionApiService : IBaseApiService
    {
        [ApiResourcePath("current")]
        IObservable<List<Objective>> GetMissions(Priority priority = Priority.Cache);
        Objective LastSelectedMission { get; set; }
    }

    public class MissionApiService : ApiServiceProvider, IMissionApiService
    {
        public IObservable<List<Objective>> GetMissions(Priority priority = Priority.Cache) => 
            this.CreateFileCacheableObservableRequest<List<Objective>, object>(new object(), priority, "missions.dat", 
                (objectuves,ex) => { 
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

        public Objective LastSelectedMission { get; set; }
    }
}

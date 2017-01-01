using System;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models;
using System.Collections.Generic;

namespace TigerApp.Shared.Services.API
{
    public interface ISurveyApiService : IBaseApiService
    {
        [ApiResourcePath("survey")]
        IObservable<List<Survey>> GetSurveys();
    }

    public class SurveyApiService : ApiServiceProvider, ISurveyApiService
    { 
        public IObservable<List<Survey>> GetSurveys() =>
            this.CreateFileCacheableObservableRequest<List<Survey>, object>(new object(), Priority.Internet, "survey.dat",
                (objectuves, ex) =>
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

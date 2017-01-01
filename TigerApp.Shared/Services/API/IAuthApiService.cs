using System;
using ReactiveUI;
using TigerApp.Shared.Models.Requests;
using TigerApp.Shared.Models.Responses;

namespace TigerApp
{
    [ApiResourcePath("auth")]
    public interface IAuthApiService : IBaseApiService
    {
        [ApiResourcePath("fb")]
        IObservable<FacebookLoginResponse> FacebookLogin(FacebookLoginRequest req);

        [ApiResourcePath("sms")]
        IObservable<SmsLoginResponse> SmsLogin(SmsLoginRequest req);

        string SmsLoginPhoneNumber { get; set; }
    }

    public class AuthApiService : ApiServiceProvider, IAuthApiService
    {
        public string SmsLoginPhoneNumber
        {
            get;
            set;
        }

        public IObservable<FacebookLoginResponse> FacebookLogin(FacebookLoginRequest req)
        {
            return this.CreateObservableRequest<FacebookLoginResponse, FacebookLoginRequest>(req);
        }

        public IObservable<SmsLoginResponse> SmsLogin(SmsLoginRequest req)
        {
            return this.CreateObservableRequest<SmsLoginResponse, SmsLoginRequest>(req, catchStatusError: (obs, ex) =>
            {
                var isUnauthorized = false;

                if (ex is BetterHttpResponseException)
                {
                    isUnauthorized = ((BetterHttpResponseException)ex).StatusCode == System.Net.HttpStatusCode.Unauthorized;
                }

                /** 
                    NOTE: 
                    401 status code is expected on the first try
                    so we must not treat as UserError
                */
                if (!isUnauthorized)
                {
                    throw ex;
                }
                else {
                    obs.OnNext(null);
                    obs.OnCompleted();
                }
            });
        }
    }
}
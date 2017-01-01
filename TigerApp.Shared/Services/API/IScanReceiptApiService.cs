using System;
using ReactiveUI;
using TigerApp.Shared.Models;
using TigerApp.Shared.Models.TrackedActions;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("receipt")]
    public interface IScanReceiptApiService : IBaseApiService
    {
        [ApiResourcePath("")]
        IObservable<object> SendReceiptData(ScanReceiptResult receiptInfo, Action onReceiptAlreadyScanned, Action onUnauthorized);
    }

    public class ScanReceiptApiService : ApiServiceProvider, IScanReceiptApiService
    {
        public IObservable<object> SendReceiptData(ScanReceiptResult receiptInfo, Action onReceiptAlreadyScanned, Action onUnauthorized) {
            return this.CreateObservableRequest<object, ScanReceiptResult>(receiptInfo, (obs, ex) =>
                {
                    var isUnauthorized = false;

                    if (ex is BetterHttpResponseException)
                    {
                        isUnauthorized = ((BetterHttpResponseException)ex).StatusCode == System.Net.HttpStatusCode.Unauthorized || ((BetterHttpResponseException)ex).StatusCode == System.Net.HttpStatusCode.Forbidden;
                    }

                    if (!isUnauthorized)
                    {
                        onReceiptAlreadyScanned?.Invoke();
                        //UserError.Throw(new UserError(ex.Message));
                    }   
                    else {
                        AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().UpdateUserLoginStatus(false);
                        onUnauthorized?.Invoke();
                    }
            });
        }
    }
}

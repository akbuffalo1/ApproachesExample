using AD.Plugins.Network.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("")]
    public interface ICouponApiService : IBaseApiService
    {
        [ApiResourcePath("coupon")]
        IObservable<List<Coupon>> GetCoupons();
        [ApiResourcePath("disposablecoupons")]
        IObservable<List<Coupon>> GetDisposableCoupons();
    }

    public class CouponApiService : ApiServiceProvider, ICouponApiService
    {
        public IObservable<List<Coupon>> GetCoupons() => 
            this.CreateFileCacheableObservableRequest<List<Coupon>, object>(new object(), Priority.Internet, "coupons.dat", 
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
        
        public IObservable<List<Coupon>> GetDisposableCoupons() =>
            this.CreateFileCacheableObservableRequest<List<Coupon>, object>(new object(), Priority.Internet, "disposablecoupons.dat",
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

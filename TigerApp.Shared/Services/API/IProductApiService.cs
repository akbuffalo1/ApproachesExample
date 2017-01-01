using AD.Plugins.Network.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using TigerApp.Shared.Models;
using TigerApp.Shared.Models.Requests;

namespace TigerApp
{
    [ApiResourcePath("product")]
    public interface IProductApiService : IBaseApiService
    {
        [ApiResourcePath("")]
        IObservable<List<Product>> GetProductList(Priority priority = Priority.Cache);

        [ApiResourcePath("{id}/vote")]
        IObservable<object> VoteProduct(ProductVoteRequest request, Action onUnauthorized);
    }

    public class ProductApiService : ApiServiceProvider, IProductApiService
    {
        public IObservable<List<Product>> GetProductList(Priority priority = Priority.Cache)
        {
            return this.CreateFileCacheableObservableRequest<List<Product>, object>(new object(), priority, "products.dat", verb: Verbs.Get);
        }

        public IObservable<object> VoteProduct(ProductVoteRequest request, Action onUnauthorized)
        {
            return this.CreateObservableRequest<object, ProductVoteRequest>(request, (obs, ex) =>
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
                    AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().UpdateUserLoginStatus(false);
                    onUnauthorized();
                }
            });
        }
    }
}

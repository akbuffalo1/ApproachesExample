using AD.Plugins.Network.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("store")]
    public interface IStoreApiService : IBaseApiService
    {
        [ApiResourcePath("")]
        IObservable<List<Store>> GetStoreList(Priority priority=Priority.Cache);
    }

    public class StoreApiService : ApiServiceProvider, IStoreApiService
    {
        public IObservable<List<Store>> GetStoreList(Priority priority = Priority.Cache)
        {
            return this.CreateFileCacheableObservableRequest<List<Store>, object>(new object(), priority, "stores.dat", verb: Verbs.Get);
        }
    }
}

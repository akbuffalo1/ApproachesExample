using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models;
using System.Linq;

namespace TigerApp.Shared.Services.API
{
    [ApiResourcePath("checkin")]
    public interface IStoreCheckInApiService : IBaseApiService
    {
        [ApiResourcePath("places")]
        IObservable<List<StoreCheckIn>> GetUserCheckIn(Priority priority = Priority.Cache);
        IObservable<List<string>> GetStoreCitiesCheckin(Priority priority = Priority.Cache);
        IObservable<List<Store>> GetStoresCheckin(Priority priority = Priority.Cache);
        Store LastCheckin { get; set; }
    }

    public class StoreCheckInApiService : ApiServiceProvider, IStoreCheckInApiService
    {
        public IObservable<List<string>> GetStoreCitiesCheckin(Priority priority = Priority.Cache)
        {
            var citiesObs = Observable.Create<List<string>>(obs =>
            {
                GetStoresCheckin(priority).SubscribeOnce(stores =>
                {
                    obs.OnNext(stores.OrderBy(s => s.Location.City.Name).Select(s => s.Location.City.Name).Distinct().ToList());
                });
                return Disposable.Empty;
            });
            citiesObs.Catch((_) => { });
            return citiesObs;
        }

        public IObservable<List<Store>> GetStoresCheckin(Priority priority = Priority.Cache)
        {
            var storesObs = Observable.Create<List<Store>>(obs =>
            {
                GetUserCheckIn(priority).SubscribeOnce(checkIns => {
                    var storeService = AD.Resolver.Resolve<IStoreApiService>();
                    storeService.GetStoreList().SubscribeOnce(stores => {
                        try
                        {
                            obs.OnNext(_filterStoresWithUserCheckIns(checkIns, stores));
                        }catch(UpdateNeededException ex){
                            storeService.GetStoreList(Priority.Internet).SubscribeOnce(updatedStores => { 
                                obs.OnNext(_filterStoresWithUserCheckIns(checkIns, updatedStores));
                            });    
                        }
                    });
                });
                return Disposable.Empty;
            });
            storesObs.Catch((_) => { });
            return storesObs;
        }

        public IObservable<List<StoreCheckIn>> GetUserCheckIn(Priority priority = Priority.Cache)
        {
            return this.CreateFileCacheableObservableRequest<List<StoreCheckIn>, object>(new object(), priority, "store_checkins.dat", verb: Verbs.Get);
        }

        private List<Store> _filterStoresWithUserCheckIns(List<StoreCheckIn> userCheckIns, List<Store> stores) {
            if (userCheckIns == null)
                return new List<Store>();
            if (stores == null || stores.Count == 0)
                throw new UpdateNeededException();
            var checkInIds = userCheckIns.Select(checkIn => checkIn.StoreId).ToList();
            var filteredStores = stores.FindAll(s => checkInIds.Contains(s.Slug)).ToList();
            if (filteredStores.Count != checkInIds.Count)
                throw new UpdateNeededException();
            return filteredStores;
        }

        public Store LastCheckin { get; set; }
    }

    public class UpdateNeededException : Exception {
        public UpdateNeededException() : base("Update Needed!!!") { 
        }
    }
}

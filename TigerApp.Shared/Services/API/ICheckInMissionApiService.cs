using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AD.Plugins.Network.Rest;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.Services.API
{
    public interface ICheckInMissionApiService:IBaseApiService
    {
        [ApiResourcePath("cities/completed")]
        IObservable<List<string>> GetStoreCitiesCheckin(Priority priority = Priority.Cache);
        [ApiResourcePath("store/completed")]
        IObservable<List<Store>> GetStoresCheckin(Priority priority = Priority.Cache);
    }

    public class CheckInMissionApiService : ApiServiceProvider, ICheckInMissionApiService
    {
        public IObservable<List<string>> GetStoreCitiesCheckin(Priority priority = Priority.Cache)
        {
            var citiesObs = Observable.Create<List<string>>(obs =>
            {
                this.CreateFileCacheableObservableRequest<CheckinCityMissionStatus, object>(new object(), priority, "checkin_cities_mission.dat", verb: Verbs.Get).SubscribeOnce(status =>
                {
                    if (status == null)
                        obs.OnNext(null);
                    else { 
                        if (status.CheckInCities != null)
                            obs.OnNext(status.CheckInCities.OrderBy(s => s).ToList());
                        else {
                            obs.OnNext(new List<string>());
                        }
                    }
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
                this.CreateFileCacheableObservableRequest<CheckinMissionStatus, object>(new object(), priority, "checkin_stores_mission.dat", verb: Verbs.Get).SubscribeOnce(status =>
                {
                    if (status == null)
                        obs.OnNext(null);
                    else {
                        if (status.StoresCheckIn == null)
                            obs.OnNext(new List<Store>());
                        else { 
                            var storeService = AD.Resolver.Resolve<IStoreApiService>();
                            storeService.GetStoreList().SubscribeOnce(stores =>
                            {
                                try
                                {
                                    obs.OnNext(_filterStoresWithUserCheckIns(status.StoresCheckIn, stores));
                                }
                                catch (UpdateNeededException ex)
                                {
                                    storeService.GetStoreList(Priority.Internet).SubscribeOnce(updatedStores =>
                                    {
                                        obs.OnNext(_filterStoresWithUserCheckIns(status.StoresCheckIn, updatedStores));
                                    });
                                }
                            });
                        }
                    }
                });
                return Disposable.Empty;
            });
            storesObs.Catch((_) => { });
            return storesObs;
        }

        private List<Store> _filterStoresWithUserCheckIns(List<string> checkinStoreIds, List<Store> stores)
        {
            if (checkinStoreIds == null)
                return new List<Store>();
            if (stores == null || stores.Count == 0)
                throw new UpdateNeededException();
            var filteredStores = stores.FindAll(s => checkinStoreIds.Contains(s.Slug)).ToList();
            if (filteredStores.Count != checkinStoreIds.Count)
                throw new UpdateNeededException();
            return filteredStores;
        }
    }
}

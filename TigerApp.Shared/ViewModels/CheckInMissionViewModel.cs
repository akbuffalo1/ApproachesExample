using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Services.API;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using TigerApp.Shared.Models;

namespace TigerApp.Shared.ViewModels
{
    public enum MissionStatus { Unknown, Completed, Failed };

    public interface ICheckInMissionViewModel : IViewModelBase
    {
        ReactiveCommand<MissionStatus> TryToCompleteMission { get; }
    }

    public class CheckInMissionViewModel : ReactiveViewModel, ICheckInMissionViewModel
    {
        [Reactive]
        public ReactiveCommand<MissionStatus> TryToCompleteMission
        {
            get;
            protected set;
        }

        private IStoreApiService _storeApi
        {
            get;
            set;
        }

        const double DEG2RAD = Math.PI / 180F;

        // Adapted from http://www.movable-type.co.uk/scripts/latlong.html
        private double DistanceBetween(Shared.Models.Location fromLocation, Shared.Models.Location toLocation)
        {
            if (fromLocation.Latitude == null || fromLocation.Longitude == null || toLocation.Latitude == null || toLocation.Longitude == null)
                return double.NegativeInfinity;

            var R = 6371e3; // metres
            var φ1 = fromLocation.Latitude.Value * DEG2RAD;
            var φ2 = toLocation.Latitude.Value * DEG2RAD;
            var Δφ = (toLocation.Latitude.Value - fromLocation.Latitude.Value) * DEG2RAD;
            var Δλ = (toLocation.Longitude.Value - fromLocation.Longitude.Value) * DEG2RAD;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static int fakeLocationIndex = 0;
        public CheckInMissionViewModel(IStoreApiService storeApi)
        {
            _storeApi = storeApi;
            TryToCompleteMission = ReactiveCommand.CreateAsyncObservable<MissionStatus>(arg =>
            {
                // User location
                var userLocation = arg as Models.Location;

                return Observable.Create<MissionStatus>((IObserver<MissionStatus> obs) =>
                {
                    _storeApi.GetStoreList().SubscribeOnce(stores =>
                    {
                        #if DEBUG
                        fakeLocationIndex = fakeLocationIndex >= stores.Count ? 0 : fakeLocationIndex;
                        userLocation = stores[fakeLocationIndex].Location;
                        fakeLocationIndex++;
                        /*stores[0].Location.Latitude = 45.0524597;
                        stores[0].Location.Longitude = 7.6565495;
                        stores[0].Address = "simply";

                        stores[1].Location.Latitude = 45.0521227;
                        stores[1].Location.Longitude = 7.659799;
                        stores[1].Address = "kido-ism";

                        stores[2].Location.Latitude = 45.0528351;
                        stores[2].Location.Longitude = 7.6566692;
                        stores[2].Address = "pulcinella";*/
                        #endif
                        double minDistance = -1;
                        Store nearestStore = null;
                        foreach (var store in stores)
                        {
                            try
                            {
                                double storeRadius = Constants.GeoFence.DefaultCheckInRadius;
                                /*
                                double storeRadius = 0;
                                if (!double.TryParse(store.Location.Radius, out storeRadius))
                                    storeRadius = Constants.GeoFence.DefaultCheckInRadius;*/

                                var distance = DistanceBetween(userLocation, store.Location);
                                System.Diagnostics.Debug.WriteLine($"Distance from {store.Address} at {store.Location.Latitude},{store.Location.Longitude} = {distance}");

                                if (distance <= storeRadius)
                                {
                                    if (minDistance < 0 || distance < minDistance) {
                                        minDistance = distance;
                                        nearestStore = store;
                                    }
                                        
                                }
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                        if (nearestStore != null) {
                            AD.Resolver.Resolve<IStoreCheckInApiService>().LastCheckin = nearestStore;
                            AD.Resolver.Resolve<ITrackedActionsApiService>().PushAction(
                                new Shared.Models.TrackedActions.StoreCheckInTrackedAction(nearestStore), null).SubscribeOnce(_ =>
                                        {
                                            obs.OnNext(MissionStatus.Completed);
                                            obs.OnCompleted();
                                        });
                        }
                        else{ 
                            obs.OnNext(MissionStatus.Failed);
                            obs.OnCompleted();
                        }
                    });
                    return Disposable.Empty;
                });
            });
        }
    }
}

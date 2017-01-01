using System;
using AD;
using AD.Plugins.Permissions;
using AD.Plugins.TripleDesAuthToken;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Services.API;
using TigerApp.Shared.Services.Platform;

namespace TigerApp.Shared.ViewModels
{
    public interface ILoadingViewModel : IViewModelBase {
        void RereshData(AuthData authData);
    }

    public class LoadingViewModel : ReactiveViewModel, ILoadingViewModel
    {
        private INotificationsService PermissionService;
        public LoadingViewModel() {
            FlagStore.Set(Constants.Flags.EXP_PAGE_TUTORIAL_SHOWN);
        }

        public void  RereshData(AuthData authData) {
            authData.AppVersion = AD.Resolver.Resolve<IApiVersionConfig>().AppVersionCode.ToString();
            AD.Resolver.Resolve<ITDesAuthStore>().SetAuthData(authData);
            AD.Resolver.Resolve<IStoreApiService>().GetStoreList(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce((stores) =>
            {
                AD.Resolver.Resolve<IProductApiService>().GetProductList(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce((_) =>
                {
                });
                var geofenceService = AD.Resolver.Resolve<IGeofenceService>();
                geofenceService.ClearAll();
                #if DEBUG
                //45.0517337,7.6587754
                //simply : 
                //    Latitude = 45.0524578,
                //    Longitude = 7.6576438
                var testPlace = new Models.TrackedActions.Place()
                {
                    PlaceId = "btz131",
                    City = "Torino",
                    Region = "Piemonte",
                    State = "Italy",
                    Latitude = 45.0517337,
                    Longitude = 7.6587754
                };
                geofenceService.RegisterPlace(testPlace);
                #endif
                foreach (var store in stores)
                    geofenceService.RegisterPlace(new Models.TrackedActions.Place(store));
                geofenceService.StartMonitoring(onEnter: (place) => {
                    if(AD.Resolver.Resolve<IProfileApiService>().IsLoggedIn)
                        AD.Resolver.Resolve<IProximityApiService>().PushProximityEvent(new Models.Proximity.ProximityEvent()
                        {
                        #if DEBUG
                            PlaceId = stores[0].Slug,//place.PlaceId,
                            Place = new Models.TrackedActions.Place(stores[0])
                        #else
                            PlaceId = place.PlaceId,
                            Place = place
                        #endif
                        }).SubscribeOnce(_ => { });
                });
            });
        }
    }
}


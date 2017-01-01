using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Services.API;

namespace TigerApp.Shared.ViewModels
{
    public interface ITwoCitiesCheckInViewModel : IViewModelBase
    {
        List<string> Cities { get; }
        void GetCheckInCities();
    }

    public class TwoCitiesCheckInViewModel : ReactiveViewModel, ITwoCitiesCheckInViewModel
    {
        [Reactive]
        public List<string> Cities { get; protected set;}

        public void GetCheckInCities() {
            AD.Resolver.Resolve<ICheckInMissionApiService>().GetStoreCitiesCheckin(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(
                cities => {
                    if(cities!=null)
                        Cities = cities.Count <= 2 ? cities : cities.Take(2).ToList();
                }
            );
        }

    }
}
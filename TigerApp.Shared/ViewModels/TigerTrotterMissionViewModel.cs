using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;

namespace TigerApp.Shared.ViewModels
{
    public interface ITigerTrotterMissionViewModel : IViewModelBase
    {
        IEnumerable<Store> StoreList { get; set; }
        void GetStoresCheckIn();
    }

    public class TigerTrotterMissionViewModel : ReactiveViewModel, ITigerTrotterMissionViewModel
    {
        [Reactive]
        public IEnumerable<Store> StoreList  { get; set;  }

        public TigerTrotterMissionViewModel()
        {
        }

        public void GetStoresCheckIn() {
            AD.Resolver.Resolve<ICheckInMissionApiService>().GetStoresCheckin(AD.Plugins.Network.Rest.Priority.Internet).SubscribeOnce(stores => {
                if (stores.Count() > 5)
                    StoreList = stores.Take(5).ToList();
                else
                    StoreList = stores.ToList();
            });
        }
    }
}

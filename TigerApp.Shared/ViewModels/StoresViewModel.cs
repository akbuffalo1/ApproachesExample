using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;

namespace TigerApp.Shared.ViewModels
{
    public interface IStoresViewModel : IViewModelBase
    {
        List<Store> Stores { get; }
        bool IsLoading { get; }

        void GetStoreList();
    }

    public class StoresViewModel : ReactiveViewModel, IStoresViewModel
    {
        [Reactive]
        public List<Store> Stores
        {
            get;
            private set;
        }

        [Reactive]
        public bool IsLoading
        {
            get;
            private set;
        }

        private readonly IStoreApiService _storeApi;

        public StoresViewModel(IStoreApiService storeApi)
        {
            _storeApi = storeApi;
        }

        public void GetStoreList()
        {
            IsLoading = true;
            _storeApi.GetStoreList()
                     .SubscribeOnce(data =>
                     {
                         Stores = data;
                         IsLoading = false;
                     });
        }
    }
}

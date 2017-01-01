using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Text.RegularExpressions;
using System.Linq;

namespace TigerApp.Shared.ViewModels
{
    public interface IStoreLocatorViewModel : IViewModelBase
    {
        List<Store> Stores { get; }
        string SearchQuery { get; set; }
        ReactiveCommand<List<Store>> SearchStores { get; }
        void GetStoreList();
    }

    public class StoreLocatorViewModel : ReactiveViewModel, IStoreLocatorViewModel
    {
        public ReactiveCommand<List<Store>> SearchStores
        {
            get;
            protected set;
        }

        [Reactive]
        public string SearchQuery
        {
            get;
            set;
        }

        [Reactive]
        public List<Store> Stores
        {
            get;
            private set;
        }

        private readonly IStoreApiService _storeApi;

        public StoreLocatorViewModel(IStoreApiService storeApi)
        {
            _storeApi = storeApi;

            SearchStores = ReactiveCommand.CreateAsyncObservable((arg) =>
            {
                var searchQueryWords = Regex.Matches(SearchQuery?.ToLower().Trim() ?? string.Empty, @"\w+")
                .Cast<Match>()
                .Select(x => x.Value)
                .ToArray();

                return Observable.Return(Stores?.FindAll(store =>
                {
                    var address = store.Address?.ToLower();
                    var city = store.Location?.City?.Name?.ToLower();
                    return searchQueryWords.All(word => $"{address},{city}".Contains(word));
                }) ?? Enumerable.Empty<Store>().ToList());
            });

            this.WhenAnyValue(x => x.SearchQuery)
            .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
            .InvokeCommand(this, x => x.SearchStores);
        }

        public void GetStoreList()
        {
            _storeApi.GetStoreList()
            .SubscribeOnce(data =>
            {
                Stores = data;
                Stores.ForEach(s =>
                {
                    s.Id = Guid.NewGuid().ToString();
                });
            });
        }
    }
}
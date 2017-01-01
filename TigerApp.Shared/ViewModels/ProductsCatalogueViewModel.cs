using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using TigerApp.Shared.Models;
using TigerApp.Shared.Models.Requests;
using System.Reactive.Subjects;
using TigerApp.Shared.Services.API;
using System.Linq;

namespace TigerApp.Shared.ViewModels
{
    public interface IProductsCatalogueViewModel : IViewModelBase
    {
        bool ShowPopup { get; }
        bool ShowTutorial { get; }
        List<Product> Products { get; }
        bool IsLoading { get; }
        bool ShowPunto { get; }
        void GetProductList();
        void VoteProduct(long productId, ProductVote vote);
        void UpdateShowPopup();
    }

    public class ProductsCatalogueViewModel : ReactiveViewModel, IProductsCatalogueViewModel
    {
        [Reactive]
        public bool ShowPopup
        {
            get;
            protected set;
        }

        public bool ShowTutorial
        {
            get
            {
                return (_profileApi.IsLoggedIn && !FlagStore.IsSet(Constants.Flags.PRODUCTS_CATALOGUE_TUTORIAL_SHOWN));
            }
        }

        [Reactive]
        public List<Product> Products
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

        public bool ShowPunto
        {
            get
            {
                return _profileApi.IsLoggedIn;
            }
        }

        private readonly IProductApiService _productApi;
        private readonly IProfileApiService _profileApi;

        public ProductsCatalogueViewModel(IProductApiService productApi, IProfileApiService profileApi)
        {
            _productApi = productApi;
            _profileApi = profileApi;
            ShowPopup = !_profileApi.IsLoggedIn && !FlagStore.IsSet(Constants.Flags.PRODUCTS_CATALOGUE_POPUP_SHOWN);
        }

        public void GetProductList()
        {
            IsLoading = true;
            _productApi.GetProductList(AD.Plugins.Network.Rest.Priority.Internet)
                       .SubscribeOnce(data =>
                       {
                           IsLoading = false;
                           var products = data.Where(p => p.Vote == null || !p.Vote.EndsWith("like", StringComparison.CurrentCulture))?.ToList();
                           products?.ForEach(p => p.Vote = p.Vote == null ? ProductVote.Ignore.ToString() : p.Vote);
                           Products = products;
                       });
        }

        public void UpdateShowPopup()
        {
            ShowPopup = !_profileApi.IsLoggedIn && !FlagStore.IsSet(Constants.Flags.PRODUCTS_CATALOGUE_POPUP_SHOWN);
        }

        public void VoteProduct(long productId, ProductVote vote)
        {
            _productApi.VoteProduct(new ProductVoteRequest { Id = productId, Vote = vote }, onUnauthorized: () =>
             {
                 ShowPopup = !FlagStore.IsSet(Constants.Flags.PRODUCTS_CATALOGUE_POPUP_SHOWN);
             })
             .Catch(ex =>
             {
                 System.Diagnostics.Debug.WriteLine(ex);
             })
            .Subscribe(data =>
            {
                //_productApi.GetProductList().SubscribeOnce((_) => { });
            });
        }
    }
}
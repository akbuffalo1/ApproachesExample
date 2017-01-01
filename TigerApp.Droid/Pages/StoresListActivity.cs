#region using

using System;
using System.Collections.Generic;
using System.Linq;
using AD;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.UI.ExpandableRecyclerView.Models;
using TigerApp.Droid.UI.Stores;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "@string/app_name")]
    public class StoresListActivity : BaseReactiveActivity<IStoresViewModel>
    {
        public StoresListActivity()
        {
            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Stores).Subscribe(stores =>
                {
                    if (stores != null)
                    {
                        Stores = stores;
                        ReloadStores();
                    }
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.IsLoading).Subscribe(isLoading =>
                {
                    if (!isLoading)
                        _progressDialog?.Dismiss();
                    else
                        _progressDialog = this.ShowTransparentProgress();

                }));

                ViewModel.GetStoreList();
            });
        }

        private static readonly string TAG = nameof(StoresListActivity);
        private StoresListAdapter _adapter;
        protected List<Store> Stores { get; private set; }

        private RecyclerView _crimeRecyclerView;
        private ImageButton _btnBack;
        private ProgressDialog _progressDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_stores_list);

            _crimeRecyclerView = FindViewById<RecyclerView>(Resource.Id.storesList);
            _crimeRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

           

            _btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
            _btnBack.Click += (sender, args) => { Finish(); };
        }

        private void OnChildClick(StoreListChildItemModel obj)
        {
            Resolver.Resolve<ILogger>().Debug(TAG, "store click: " + obj.Title);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);

            _adapter.OnRestoreInstanceState(savedInstanceState);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            _adapter.OnSaveInstanceState(outState);
        }

        private void ReloadStores()
        {
            var adapterList = Stores.GroupBy(s => s.Location.City.Region.Name)
                .Select(group =>
                    new StoreListItemModel(group.Key,
                        group.ToList().Select(g => new StoreListChildItemModel
                        {
                            Title = g.Location.City.Name,
                            Contact = $"{g.Address} - T {g.Phone}",
                            WorkingHours = string.Join("\n", g.OpeningHoursText.Split(';').Select(s => s.Trim()))
                        }).ToList())
                )
                .ToList();

            _adapter = new StoresListAdapter(this, adapterList.Cast<IParentObject>().ToList());
            _adapter.ParentAndIconExpandOnClick = true;
            _adapter.OnChildClickEvent += OnChildClick;

            _crimeRecyclerView.SetAdapter(_adapter);
        }

    }
}
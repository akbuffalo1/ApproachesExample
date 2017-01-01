
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.UI.Coupons;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "TigerTrotterMissionActivity")]
    public class TigerTrotterMissionActivity : BaseReactiveActivity<ITigerTrotterMissionViewModel>
    {
        DynamicLayout checkinListView;
        ImageView _btnBack;

        public TigerTrotterMissionActivity()
        {
            this.WhenActivated(dispose =>
            {
                dispose(ViewModel.WhenAnyValue(vm => vm.StoreList).Where(stores => stores != null).Subscribe((stores) =>
                {
                    checkinListView.Adapter = new CityListAdapter(this, stores.ToList());
                }));
                ViewModel.GetStoresCheckIn();
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tiger_trotter_mission);

            checkinListView = FindViewById<DynamicLayout>(Resource.Id.checkinListView);
            _btnBack = FindViewById<ImageView>(Resource.Id.btnBack);
            _btnBack.Click += (sender, e) => Finish();
        }
    }

    public class CityListAdapter : IViewCreator
    {
        public const int DATA_TO_DISPLAY = 5;

        private Context _context;
        private IList<Store> _models;
        private IList<DataWrapper> dataWrapper = new List<DataWrapper>();

        public CityListAdapter(Context context, List<Store> models)
        {
            _context = context;
            _models = models;

            dataWrapper = new List<DataWrapper>();
            for (int counter = 0; counter < DATA_TO_DISPLAY; counter++)
            { 
                if (counter < _models.Count)
                {
                    dataWrapper.Add(new DataWrapper
                    {
                        Data = _models[counter],
                        TemplateId = Resource.Layout.layout_trotter_mission_city_list_item
                    });
                }
                else 
                {
                    dataWrapper.Add(new DataWrapper
                    {
                        Data = null,
                        TemplateId = Resource.Layout.layout_trotter_mission_city_list_item_empty
                    });
                }
            }
        }

        public View CreateView(int position)
        {
            var current = dataWrapper[position];
            var view = LayoutInflater.From(_context).Inflate(current.TemplateId, null);
            if (!current.IsPlaceHolder)
            {
                view.FindViewById<TextView>(Resource.Id.tvStoreName).Text = current.Data.Location.City.Name;
                view.FindViewById<TextView>(Resource.Id.tvStoreAddress).Text = current.Data.Address;
            }
            if (position == GetCount() - 1) 
            {
                view.FindViewById<ImageView>(Resource.Id.ivCityListDivider).Visibility = ViewStates.Gone;
            }

            return view;
        }

        public int GetCount()
        {
            return dataWrapper.Count;
        }

        internal class DataWrapper
        {
            public int TemplateId { get; set; }
            public Store Data { get; set; }
            public bool IsPlaceHolder
            {
                get
                {
                    return Data == null;
                }
            }
        }
    }
}


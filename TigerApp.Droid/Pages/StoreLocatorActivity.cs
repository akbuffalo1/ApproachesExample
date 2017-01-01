#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AD;
using AD.Plugins.Permissions;
using AD.Plugins.PhoneCall;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.UI;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using GLocation = Android.Locations.Location;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity]
    public class StoreLocatorActivity : BaseReactiveActivity<IStoreLocatorViewModel>, IOnMapReadyCallback
    {
        static string Tag = nameof(StoreLocatorActivity);

        private const float MAP_ZOOM = 12;

        private StoresLocatorAdapter _adapter;
        private Button _btnPhone;
        private SnappingLinearLayoutManager _layoutManager;
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private readonly Dictionary<Marker, Store> _markers = new Dictionary<Marker, Store>();
        private GLocation _myLocation;
        private Marker _selectedMarker;
        private Store _selectedStore;
        private ViewGroup _storeInfoLayout;
        private RecyclerView _storesList;
        private TextView _txtAddress;
        private TextView _txtCaption;
        private TextView _txtDistance;
        private EditText _txtInpuLoc;
        private TextView _txtName;
        private TextView _txtPhoneNumber;
        private TextView _txtTown;
        private TextView _txtWorkingHours;
        private ImageView _btnBack;
        private bool _mapReady;
        private bool _permissionDialogShowed;
        private ImageButton _btnLocator;
        private TextView _txtStoresNotFound;

        protected List<Store> Stores { get; private set; }

        public StoreLocatorActivity()
        {
            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Stores).Subscribe(stores =>
                {
                    if (stores != null)
                    {
                        Stores = stores;
                        Stores.ForEach(s => { s.Id = new Random().Next(230000004, 999999999).ToString(); });
                        RefreshStoreMarkers();
                    }
                }));

                dis(this.WhenAnyValue(x => x._txtInpuLoc.Text).BindTo(ViewModel, x => x.SearchQuery));

                dis(this.ViewModel.SearchStores.Subscribe(results =>
                {
                    _txtStoresNotFound.Visibility = (results.Count == 0) ? ViewStates.Visible : ViewStates.Invisible;
                    Stores = results;
                    RefreshStoreMarkers();
                    RefreshMapBoundsAndZoom();
                }));

                ViewModel.GetStoreList();
            });
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_store_locator);

            SetupMapIfNeeded();

            _storesList = FindViewById<RecyclerView>(Resource.Id.storesList);
            _storeInfoLayout = FindViewById<ViewGroup>(Resource.Id.storeInfoLayout);

            _txtInpuLoc = FindViewById<EditText>(Resource.Id.txtInpuLoc);
            _txtCaption = FindViewById<TextView>(Resource.Id.txtCaption);
            _txtStoresNotFound = FindViewById<TextView>(Resource.Id.txtStoresNotFound);
            _txtStoresNotFound.Visibility = ViewStates.Invisible;

            _txtTown = FindViewById<TextView>(Resource.Id.txtTown);
            _txtAddress = FindViewById<TextView>(Resource.Id.txtAddress);
            _txtDistance = FindViewById<TextView>(Resource.Id.txtDistance);
            _txtName = FindViewById<TextView>(Resource.Id.txtName);
            _txtWorkingHours = FindViewById<TextView>(Resource.Id.txtWorkingHours);
            _txtPhoneNumber = FindViewById<TextView>(Resource.Id.txtPhoneNumber);
            _btnPhone = FindViewById<Button>(Resource.Id.btnPhone);
            _btnBack = FindViewById<ImageView>(Resource.Id.btnBack);
            _btnLocator = FindViewById<ImageButton>(Resource.Id.btnLocator);

            _btnLocator.Click += (sender, args) =>
            {
                SetStoreInfoVisible(false);
                Stores = ViewModel.Stores;
                RefreshStoreMarkers();
                MoveMapToMyLocation();
            };

            _txtInpuLoc.Click += (sender, args) => { SetStoreInfoVisible(false); };
            _txtInpuLoc.FocusChange += (sender, args) => { if(args.HasFocus)SetStoreInfoVisible(false); };
            _txtInpuLoc.SetImeActionLabel("Cerca",Android.Views.InputMethods.ImeAction.Done);
            _txtInpuLoc.SetImeActionLabel("Cerca",Android.Views.InputMethods.ImeAction.Next);            
            _txtInpuLoc.KeyPress += (object sender, View.KeyEventArgs e) => {
                if (e.Event.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down) {
                    Window.SetSoftInputMode(SoftInput.StateHidden);
                }
            };          
            //_txtInpuLoc.KeyListener = new OnKeyListener()

            _btnBack.Click += (sender, args) => { OnBackPressed(); };
            _btnPhone.Click += OnPhoneClick;

            _layoutManager = new SnappingLinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            _storesList.SetLayoutManager(_layoutManager);
            _adapter = new StoresLocatorAdapter(this, _storesList);
            _storesList.SetAdapter(_adapter);
            _adapter.OnItemClik += OnItemClick;

            #region BUG

            //strange BUG: keybord always on top with MapsFragment
            //currently bug dissapeared, dont know reason and I forgot to log Android API number with bug :)
            //checked API 4.1.1, 4.2.2, 4.4.4, 5.1.0, 6.0.0
            //_txtInpuLoc.InputType = 0;
            //Task.Delay(350).ContinueWith(o => { RunOnUiThread(() => _txtInpuLoc.InputType = InputTypes.ClassText); });

            #endregion

            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustNothing);

            PlatformUtil.IsGooglePlayServicesAvailable(this);
        }

        private void OnPhoneClick(object sender, EventArgs e)
        {
            var callTask = AD.Resolver.Resolve<IPhoneCallTask>();
            callTask.MakePhoneCall(_selectedStore.Name, _selectedStore.Phone.Replace("/", string.Empty));
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (_mapReady)
                EnableMyLocation();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;

            EnableMyLocation();

            _map.MarkerClick += OnMarkerClick;

            _mapReady = true;
        }

        private void RefreshStoreMarkers()
        {
            if (Stores != null && _myLocation != null)
            {
                foreach (var store in Stores)
                {
                    store.Location.Latitude = store.Location.Latitude != null ? store.Location.Latitude : 0;
                    store.Location.Longitude = store.Location.Longitude != null ? store.Location.Longitude : 0;
                }
                Stores.ForEach(
                    s =>
                        s.DistanceInMeters =
                            (int)
                            (new GLocation(s.Slug)
                            {
                                Latitude = s.Location.Latitude.Value,
                                Longitude = s.Location.Longitude.Value
                            }).DistanceTo(_myLocation));
                Stores.Sort((x, y) => x.DistanceInMeters - y.DistanceInMeters);

                _adapter.Stores = Stores;

                SetMarkers();
                SelectStore(Stores[0]);

                _map.MyLocationChange -= OnMyLocationChanged;
            }
        }

        private void RefreshMapBoundsAndZoom()
        {
            if (Stores == null || _myLocation == null || !_mapReady)
                return;

            LatLngBounds.Builder builder = new LatLngBounds.Builder();
            _markers.ForEach(pair =>
            {
                var marker = pair.Key;
                builder.Include(marker.Position);
            });

            LatLngBounds bounds = builder.Build();

            int padding = ScreenUtils.Dp2Px(this, 20); // offset from edges of the map in pixels
            CameraUpdate cu = CameraUpdateFactory.NewLatLngBounds(bounds, padding);
            _map.MoveCamera(cu);
        }

        private void OnItemClick(Store store)
        {
            SetStoreInfoVisible(true);

            _txtCaption.Text = Resources.GetString(Resource.String.store_info);

            if (_selectedStore != store)
                SelectStore(store,true);
        }

        public override void OnBackPressed()
        {
            if (_storeInfoLayout.Visibility == ViewStates.Visible)
            {
                SetStoreInfoVisible(false);
                _txtCaption.Text = Resources.GetString(Resource.String.store_locator);
                return;
            }
            base.OnBackPressed();
        }

        private void SetupMapIfNeeded()
        {
            _mapFragment = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
            _mapFragment.GetMapAsync(this);
        }

        private async void EnableMyLocation()
        {
            var perm = AD.Resolver.Resolve<IPermissions>();

            var status = await perm.CheckPermissionStatusAsync(Permission.Location);

            if (status != PermissionStatus.Granted)
            {
                if (!_permissionDialogShowed)
                {
                    _permissionDialogShowed = true;
                    AlertDialog alertDialog = null;
                    var alert = new AlertDialog.Builder(this);
                    alert.SetMessage(Resources.GetString(Resource.String.msg_loc_you_have_to_enable));
                    alert.SetPositiveButton("Ok", (sender, e) =>
                    {
                        alertDialog.Dismiss();
                        perm.RequestPermissionsAsync(Permission.Location);
                    });
                    alertDialog = alert.Show();
                    /*DialogUtil.ShowAlert(this, Resources.GetString(Resource.String.msg_loc_you_have_to_enable), "Ok",
                        () =>
                        {
                            perm.RequestPermissionsAsync(Permission.Location);
                        });*/
                }
                return;
            }

            var locationManager = (LocationManager) GetSystemService(Context.LocationService);

            if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                AlertDialog alertDialog = null;
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("GPS non trovato!");
                alert.SetMessage(Resources.GetString(Resource.String.msg_location_disabled_please_enable));
                alert.SetPositiveButton("Vai alle impostazioni", (sender, e) =>
                {
                    alertDialog.Dismiss();
                    var callGpsSettingIntent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    StartActivity(callGpsSettingIntent);
                });
                alertDialog = alert.Show();
                /*
                DialogUtil.ShowConfirmation(this, Resources.GetString(Resource.String.msg_location_disabled_please_enable),
                    () =>
                    {
                        var callGpsSettingIntent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                        StartActivity(callGpsSettingIntent);
                    });*/
            }

            //BUG After permission grant GoogleMap does not updates location, should restart Activity or use ILocationWatcher from AD.Plugins

            _map.MyLocationChange += OnMyLocationChanged;
            _map.MyLocationEnabled = true;
            _map.UiSettings.MyLocationButtonEnabled = false;
        }

        private void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            //next lines not works
            // _markers.TryGetValue(e.Marker, out store); not works
            //_markers.ContainsKey(e.Marker); returns false O_o
            var store = _markers.FirstOrDefault(pair => pair.Key.Id == e.Marker.Id).Value;
            SelectStore(store);
        }

        private void SelectStore(Store store, bool moveCamera = false)
        {
            if (_selectedMarker != null)
            {
                _selectedMarker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_store_location_marker));
            }

            _selectedStore = store;

            _selectedMarker = _markers.FirstOrDefault(x => x.Value.Equals(_selectedStore)).Key;

            _selectedMarker.SetIcon(
                BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_store_location_marker_selected));

            _adapter.SelectStore(_selectedStore);

            _txtTown.Text = store.Location.City.Name;
            _txtAddress.Text = store.Address;
            _txtWorkingHours.Text = string.Join("\n", store.OpeningHoursText.Split(';').Select(s => s.Trim()));
            _txtPhoneNumber.Text = store.Phone;
            _txtDistance.Text = store.DistanceInMeters > 999
                    ? $"{((float)store.DistanceInMeters / 1000).ToString("n1")}km"
                    : $"{store.DistanceInMeters}m";
            if (_txtDistance.Text.EndsWith(",0km", StringComparison.CurrentCulture))
                _txtDistance.Text = _txtDistance.Text.Replace(",0", string.Empty);

            if(moveCamera)
                _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(
                    new LatLng(store.Location.Latitude.Value, store.Location.Longitude.Value), MAP_ZOOM));
        }

        private void OnMyLocationChanged(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            _myLocation = e.Location;
            MoveMapToMyLocation();
            RefreshStoreMarkers();
        }

        private void MoveMapToMyLocation()
        {
            _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(
                new LatLng(_myLocation.Latitude, _myLocation.Longitude), MAP_ZOOM));
        }

        private void SetStoreInfoVisible(bool visible)
        {
            if (visible)
            {
                _storesList.Visibility = ViewStates.Gone;
                _storeInfoLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                _storesList.Visibility = ViewStates.Visible;
                _storeInfoLayout.Visibility = ViewStates.Gone;
            }
        }

        private void SetMarkers()
        {
            _map.Clear();
            _markers.Clear();
            _selectedMarker = null;

            Stores.ForEach(store =>
            {
                var mo = new MarkerOptions();
                mo.SetPosition(new LatLng(store.Location.Latitude.Value, store.Location.Longitude.Value));
                mo.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_store_location_marker));
                mo.SetTitle(store.Name);

                var marker = _map.AddMarker(mo);
                _markers.Add(marker, store);
            });
        }

        private class StoreViewHolder : RecyclerView.ViewHolder
        {
            public readonly ImageView icLocator;
            public readonly View itemView;
            public readonly TextView txtAddress;
            public readonly TextView txtDistance;
            public readonly TextView txtTown;

            public StoreViewHolder(View itemView) : base(itemView)
            {
                this.itemView = itemView;
                icLocator = itemView.FindViewById<ImageView>(Resource.Id.icLocator);
                txtTown = itemView.FindViewById<TextView>(Resource.Id.txtTown);
                txtAddress = itemView.FindViewById<TextView>(Resource.Id.txtAddress);
                txtDistance = itemView.FindViewById<TextView>(Resource.Id.txtDistance);
            }
        }

        private class StoresLocatorAdapter : RecyclerView.Adapter
        {
            private readonly Context _context;
            private readonly RecyclerView _recycler;
            private List<Store> _stores;
            private Store _selectedStore;

            public StoresLocatorAdapter(Context context, RecyclerView recycler)
            {
                _stores = new List<Store>();
                _context = context;
                _recycler = recycler;
            }

            public override int ItemCount
            {
                get { return _stores.Count(); }
            }

            public List<Store> Stores
            {
                get { return _stores; }
                set
                {
                    _stores = value;
                    NotifyDataSetChanged();
                }
            }

            public event Action<Store> OnItemClik;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var vh = holder as StoreViewHolder;
                var store = _stores.ElementAt(position);
                var selected = store == _selectedStore;
                vh.icLocator.SetImageResource(
                    selected
                        ? Resource.Drawable.ic_store_location_marker_selected
                        : Resource.Drawable.ic_store_location_unselected);
                vh.txtTown.Text = store.Location.City.Name;
                vh.txtAddress.Text = store.Address;
                vh.txtDistance.Text = store.DistanceInMeters > 999
                    ? $"{((float)store.DistanceInMeters / 1000).ToString("n1")}km"
                    : $"{store.DistanceInMeters}m";
                if (vh.txtDistance.Text.EndsWith(",0km",StringComparison.CurrentCulture))
                    vh.txtDistance.Text = vh.txtDistance.Text.Replace(",0",string.Empty);

                var txtColor = selected
                    ? _context.Resources.GetColor(Resource.Color.store_locator_selected_item_text)
                    : _context.Resources.GetColor(Resource.Color.store_locator_unselected_item_text);

                vh.itemView.SetBackgroundColor(
                    selected
                        ? _context.Resources.GetColor(Resource.Color.store_locator_selected_list_item_back)
                        : _context.Resources.GetColor(Resource.Color.store_locator_unselected_list_item_back)
                );

                vh.txtTown.SetTextColor(txtColor);
                vh.txtAddress.SetTextColor(txtColor);
                vh.txtDistance.SetTextColor(txtColor);
                vh.itemView.Click += (sender, args) => { OnItemClik?.Invoke(_stores.ElementAt(vh.AdapterPosition)); };
            }

            public void SelectStore(Store store)
            {
                if (_selectedStore != null)
                    NotifyItemChanged(_stores.IndexOf(_selectedStore));

                _selectedStore = store;
                var pos = _stores.IndexOf(_selectedStore);
                NotifyItemChanged(pos);

                _recycler.ScrollToPosition(pos);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.item_store_locator_list, parent, false);

                var vh = new StoreViewHolder(itemView);

                return vh;
            }
        }
    }
    /*public class SearchKeyListner : Java.Lang.Object,Android.Text.Method.IKeyListener
    {
       
        public InputTypes InputType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ClearMetaKeyState(View view, IEditable content, [GeneratedEnum] MetaKeyStates states)
        {
            throw new NotImplementedException();
        }

        public bool OnKeyDown(View view, IEditable text, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            throw new NotImplementedException();
        }

        public bool OnKeyOther(View view, IEditable text, KeyEvent e)
        {
            throw new NotImplementedException();
        }

        public bool OnKeyUp(View view, IEditable text, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            throw new NotImplementedException();
        }
    }*/
}
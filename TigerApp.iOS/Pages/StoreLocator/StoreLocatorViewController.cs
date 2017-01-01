using AD;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.StoreLocator
{
    public partial class StoreLocatorViewController : BaseReactiveViewController<IStoreLocatorViewModel>, IUICollectionViewDelegate
    {
        void UserLocationBtn_TouchUpInside(object sender, EventArgs e)
        {
            activityIndicator.StartAnimating();
            var locationService = AD.Resolver.Resolve<Shared.Services.Platform.ILocationService>();
            locationService.CurrentLocationAsync().ToObservable().SubscribeOnce(userLocation =>
            {
                InvokeOnMainThread(() =>
                {
                    activityIndicator.StopAnimating();
                    mapView.SetCenterCoordinate(new CLLocationCoordinate2D(userLocation.Latitude.Value, userLocation.Longitude.Value), true);
                    mapView.Camera.Altitude = ZOOMED_CAMERA_ALTITUDE;
                    Stores = ViewModel.Stores;
                    RefreshStoreAnnotations();
                });
            });
        }

        void ChiamaButton_TouchUpInside(object sender, EventArgs e)
        {
            AD.Resolver.Resolve<AD.IPhoneCallTask>().MakePhoneCall(null, _selectedStore?.Phone.Replace("/", string.Empty).Replace(" ",string.Empty));
        }

        private const float NORMAL_CAMERA_ALTITUDE = 2200F;
        private const float ZOOMED_CAMERA_ALTITUDE = 2000F;
        private Store _selectedStore;
        public StoreCollectionSource collectionSource { get; private set; }
        protected nfloat storeCollectionViewHeight { get; private set; }
        protected nfloat infoSubviewHeight { get; private set; }

        protected List<Store> Stores { get; private set; }
        protected IEnumerable<StoreAnnotation> StoreAnnotations { get; private set; }
        protected MapDelegate mapDelegate { get; private set; }

        private static readonly UIImage maskImage = UIImage.FromBundle("store_loc_06");
        private static readonly UIImage scaledSeparatorImage = UIImage.FromBundle("store_loc_05").Scale(new CGSize(4, 30));

        partial void OnBackButtonClick(NSObject sender)
        {
            DismissViewController(true, null);
        }

        [Export("scrollViewDidScroll:")]
        public void Scrolled(UIScrollView scrollView)
        {
            storesCollectionView.MaskView.Frame = new CGRect(storesCollectionView.ContentOffset.X, 0, storesCollectionView.MaskView.Frame.Width, storesCollectionView.MaskView.Frame.Height);
        }

        private void MapDelegate_OnUserLocationUpdated(object sender, ValueEventArgs<MKUserLocation> e)
        {
            var myLocation = mapView.UserLocation.Coordinate;
            mapView.SetCenterCoordinate(myLocation, false);
            RefreshStoreAnnotations();
        }

        private void RefreshStoreAnnotations()
        {
            if (Stores != null && mapView.UserLocation.Location != null)
            {
                Stores.ForEach(s => s.DistanceInMeters = (int)new CLLocation(s.Location.Latitude.Value, s.Location.Longitude.Value).DistanceFrom(mapView.UserLocation.Location));

                if (StoreAnnotations != null)
                {
                    mapView.RemoveAnnotations(mapView.Annotations);
                }

                Stores.Sort((x, y) => x.DistanceInMeters - y.DistanceInMeters);
                StoreAnnotations = Stores.Select(store => new StoreAnnotation(store));

                RefreshStoresCollectionSource();
                mapView.AddAnnotations(StoreAnnotations.ToArray());

                mapDelegate.OnUserLocationUpdated -= MapDelegate_OnUserLocationUpdated;
            }
        }

        private void MapDelegate_OnStoreAnnotationDeselected(object sender, AD.ValueEventArgs<string> e)
        {
            var storeAnn = StoreAnnotations.FirstOrDefault(an => an.StoreId == e.Value);
            if (storeAnn == null)
                return;

            mapView.RemoveAnnotation(storeAnn);
            mapView.AddAnnotation(storeAnn);
            mapView.SelectAnnotation(storeAnn, false);
        }

        private void MapDelegate_OnStoreAnnotationSelected(object sender, ValueEventArgs<string> e)
        {
            var store = Stores.FirstOrDefault(s => s.Id == e.Value);
            var storeIndex = Stores.IndexOf(store);
            storesCollectionView.SelectItem(NSIndexPath.FromItemSection(storeIndex, 0), true, UICollectionViewScrollPosition.CenteredHorizontally);
        }

        private void OnMapViewClick()
        {
            mapView.ShowAnnotations(mapView.Annotations, true);

            if (InfoSubview.Alpha == 0)
                return;

            infoCollectionViewHeightConstraint.Constant = storeCollectionViewHeight;
            storeCollectionViewHeightConstraint.Constant = storeCollectionViewHeight;
            storesCollectionView.Hidden = false;

            UIView.AnimateNotify(0.5F, () =>
            {
                storesCollectionView.Alpha = 1F;
                InfoSubview.Alpha = 0F;
                View.LayoutIfNeeded();
            }, finished =>
            {
                InfoSubview.Hidden = true;
            });
        }

        [Export("collectionView:shouldSelectItemAtIndexPath:")]
        public bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        [Export("collectionView:shouldDeselectItemAtIndexPath:")]
        public bool ShouldDeselectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            _selectedStore = Stores.ElementAt(indexPath.Row);
            var storeAnn = StoreAnnotations.First(an => an.StoreId == _selectedStore.Id);

            infoCityLabel.Text = _selectedStore.Location.City.Name;
            infoStreetLabel.Text = _selectedStore.Address;
            infoDistanceLabel.Text = _selectedStore.DistanceInMeters > 999
                    ? $"{((float)_selectedStore.DistanceInMeters / 1000).ToString("n1")}km"
                    : $"{_selectedStore.DistanceInMeters}m";
            if (infoDistanceLabel.Text.EndsWith(",0km", StringComparison.CurrentCulture))
                infoDistanceLabel.Text = infoDistanceLabel.Text.Replace(",0", string.Empty);
            infoTelephone.Text = _selectedStore.Phone;

            infoWorkhours.Text = string.Join("\n", _selectedStore.OpeningHoursText.Split(';').Select(s => s.Trim()));

            infoCollectionViewHeightConstraint.Constant = infoSubviewHeight;
            storeCollectionViewHeightConstraint.Constant = 0;
            InfoSubview.Hidden = false;
            var center = new CLLocationCoordinate2D((double)_selectedStore.Location.Latitude, (double)_selectedStore.Location.Longitude);
            mapView.SetCenterCoordinate(center, false);
            mapView.Camera.Altitude = ZOOMED_CAMERA_ALTITUDE;
            mapView.RemoveAnnotation(storeAnn);
            mapView.AddAnnotation(storeAnn);
            mapView.SelectAnnotation(storeAnn, true);

            UIView.AnimateNotify(0.5F, () =>
            {
                InfoSubview.Alpha = 1F;
                storesCollectionView.Alpha = 0F;
                View.LayoutIfNeeded();
            }, finished =>
            {
                storesCollectionView.Hidden = true;
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupInformationView();
            SetupStoresCollectionView();
            SetupMapView();

            chiamaButton.TouchUpInside += ChiamaButton_TouchUpInside;
            searchField.ReturnKeyType = UIReturnKeyType.Search;
            searchField.EditingDidEndOnExit += (sender, e) => searchField.ResignFirstResponder();
            userLocationBtn.TouchUpInside += UserLocationBtn_TouchUpInside;
        }

        private void SetupMapView()
        {
            mapDelegate = new MapDelegate();
            mapView.Delegate = mapDelegate;
            mapView.ShowsUserLocation = true;

            mapDelegate.OnStoreAnnotationSelected += MapDelegate_OnStoreAnnotationSelected;
            mapDelegate.OnStoreAnnotationDeselected += MapDelegate_OnStoreAnnotationDeselected;
            mapDelegate.OnUserLocationUpdated += MapDelegate_OnUserLocationUpdated;

            mapView.AddGestureRecognizer(new UITapGestureRecognizer(OnMapViewClick));
            mapView.Camera.Altitude = NORMAL_CAMERA_ALTITUDE;
        }

        private void SetupInformationView()
        {
            infoSubviewHeight = InfoSubview.Frame.Height;

            infoView.Add(InfoSubview);

            InfoSubview.Alpha = 0;
            InfoSubview.TranslatesAutoresizingMaskIntoConstraints = false;
            InfoSubview.TopAnchor.ConstraintEqualTo(infoView.TopAnchor).Active = true;
            InfoSubview.LeadingAnchor.ConstraintEqualTo(infoView.LeadingAnchor).Active = true;
            InfoSubview.TrailingAnchor.ConstraintEqualTo(infoView.TrailingAnchor).Active = true;
            InfoSubview.BottomAnchor.ConstraintEqualTo(infoView.BottomAnchor).Active = true;

            var maskView = new UIImageView(maskImage);
            infoView.MaskView = maskView;
            InfoSubview.MaskView = new UIImageView(maskImage);
            phoneSeparatorView.BackgroundColor = UIColor.FromPatternImage(scaledSeparatorImage);
        }

        private void SetupStoresCollectionView()
        {
            var maskView = new UIImageView(maskImage);

            collectionSource = new StoreCollectionSource();

            storesCollectionView.DataSource = collectionSource;
            storesCollectionView.Delegate = this;

            storeCollectionViewHeight = storeCollectionViewHeightConstraint.Constant;

            storesCollectionView.Bounces = false;
            storesCollectionView.AllowsSelection = true;

            storesCollectionView.BackgroundView = null;
            storesCollectionView.MaskView = maskView;
            storesCollectionView.BackgroundColor = UIColor.Clear;

            storesCollectionView.RegisterNibForCell(StoreCollectionViewCell.Nib, StoreCollectionViewCell.ReusableIdentifier);
        }

        private void RefreshStoresCollectionSource()
        {
            collectionSource.Stores = Stores;
            storesCollectionView.ReloadData();
        }

        [Export("collectionView:willDisplayCell:forItemAtIndexPath:")]
        public void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
        {
            cell.WithType<StoreCollectionViewCell>((c) => c.Bind(Stores.ElementAt(indexPath.Row)));
        }

        public StoreLocatorViewController()
        {
            TransitioningDelegate = TransitionManager.Right;

            this.WhenActivated(dis =>
            {
                dis(this.WhenAnyValue(x => x.searchField.Text).BindTo(ViewModel, x => x.SearchQuery));

                dis(this.ViewModel.SearchStores.Subscribe(results =>
                {
                    // Show label if result is an empty list.
                    empyStoreListLbl.Hidden = results.Count != 0;

                    Stores = results;
                    RefreshStoreAnnotations();
                    OnMapViewClick();
                }));

                dis(ViewModel.WhenAnyValue(vm => vm.Stores).Subscribe(stores =>
                {
                    if (stores != null)
                    {
                        Stores = stores.Where(s => s.Location.Latitude.HasValue && s.Location.Longitude.HasValue).ToList();
                        RefreshStoreAnnotations();
                    }
                }));

                ViewModel.GetStoreList();
            });
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            View.EndEditing(true);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;
using TigerApp.Shared.Models.TrackedActions;
using TigerApp.Shared.Services.Platform;
using UIKit;

namespace TigerApp.iOS.Services.Platform
{
    public class ProximityService : NSObject, IGeofenceService, ICLLocationManagerDelegate
    {
        private List<CLCircularRegion> _geofences;
        private List<Place> _places;
        private List<GeofenceInfo> _infos;
        private CLLocationManager _locationManager;
        private Action<Place> _onEnter;
        private Action<Place> _onExit;
        public CLLocation CurrentLocation 
        {
            get;
            protected set;
        }

        public bool IsMonitoring { 
            get {
                if (_locationManager == null || _locationManager.MonitoredRegions == null)
                    return false;
                return _locationManager.MonitoredRegions.Count > 0;
            }
        }

        public ProximityService()
        {
            _locationManager = new CLLocationManager();
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                _locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
                _locationManager.DistanceFilter = CLLocationDistance.FilterNone;
                _locationManager.RequestWhenInUseAuthorization(); // works in foreground
            }
            _locationManager.Delegate = this;
            _locationManager.RequestAlwaysAuthorization();
        }

        public void ClearAll()
        {
            StopMonitoring();
            _geofences?.Clear();
            _geofences = null;
            _places?.Clear();
            _places = null;
        }

        public void RegisterPlace(Place place)
        {
            if (_geofences == null)
                _geofences = new List<CLCircularRegion>();
            if (_places == null)
                _places = new List<Place>();
            _places.Add(place);
            _geofences.Add(_createRegionFromPlace(place));
        }

        public void SaveAll()
        {
            
        }

        public bool StartMonitoring(Action<Place> onEnter=null, Action<Place> onExit=null)
        {
            if(!CLLocationManager.IsMonitoringAvailable(typeof(CLCircularRegion)))
            {
                    //showAlert(withTitle: "Error", message: "Geofencing is not supported on this device!")
                return false;
            }
            // 2
            if(CLLocationManager.Status != CLAuthorizationStatus.AuthorizedAlways) {
                //showAlert(withTitle: "Warning", message: "Your geotification is saved but will only be activated once you grant Geotify permission to access the device location.")
                return false;
            }
            _onEnter = onEnter;
            _onExit = onExit;
            CurrentLocation = _locationManager.Location;
            UpdateActiveGeofences(CurrentLocation);
            return true;
        }

        public bool StopMonitoring()
        {
            try
            {
                _onEnter = null;
                _onExit = null;
                foreach (var monitoredRegion in _locationManager.MonitoredRegions) {
                    var geofence = monitoredRegion as CLCircularRegion;
                    if(geofence != null)
                        _locationManager.StopMonitoring(geofence);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private CLCircularRegion _createRegionFromPlace(Place place) {
            return new CLCircularRegion(center: new CLLocationCoordinate2D(place.Latitude, place.Longitude),
                                        radius: Shared.Constants.GeoFence.DefaultRadius,
                                        identifier: place.PlaceId);
        }

        private Place _getPlaceFromRegion(CLRegion region) {
            return _places.Find(p => p.PlaceId.Equals(region.Identifier));
        }

        [Export("locationManager:monitoringDidFailForRegion:withError:")]
        public void MonitoringFailed(CLLocationManager manager, CLRegion region, NSError error)
        {
            Console.WriteLine(string.Format("Monitoring failed for region : {0} with error : {1}",region.Identifier,error));
            //throw new System.NotImplementedException();
        }

        [Export("locationManager:didFailWithError:")]
        public void Failed(CLLocationManager manager, NSError error)
        {
            Console.WriteLine(string.Format("Monitoring failed!!! : {0}", error));
        }

        [Export("locationManager:didEnterRegion:")]
        public void RegionEntered(CLLocationManager manager, CLRegion region)
        {
            #if DEBUG
            //TODO REMOVE
            var alert = new UIAlertView();
            alert.Title = "GEOFENCE";
            alert.Message = "SIMPLY";
            alert.AddButton("OK");
            alert.DismissWithClickedButtonIndex(0, true);
            alert.Show();
            /// 
            #endif
            _onEnter?.Invoke(_getPlaceFromRegion(region));
        }

        [Export("locationManager:didExitRegion:")]
        public void RegionLeft(CLLocationManager manager, CLRegion region)
        {
            _onExit?.Invoke(_getPlaceFromRegion(region));
        }

        [Export("locationManager:didUpdateLocations:")]
        public void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            if (locations.Length > 0) {
                var userLocation = locations[0];
                CurrentLocation = userLocation;
                if(IsMonitoring)
                    UpdateActiveGeofences(userLocation);
            }
        }

        private void UpdateActiveGeofences(CLLocation location) {
            if (location == null)
                return;
            if (_infos == null)
                _infos = new List<GeofenceInfo>();
            //search for nearest 20 geofences
            foreach(var geofence in _geofences) {
                var distance = location.DistanceFrom(new CLLocation(geofence.Center.Latitude,geofence.Center.Longitude));
                var info = _infos.Find(i => i.GeofenceId.Equals(geofence.Identifier));
                if (info == null) { 
                    info = new GeofenceInfo(geofence.Identifier);
                    _infos.Add(info);
                }
                info.DistanceFromUser = distance;
            }
            _infos = _infos.OrderBy(i => i.DistanceFromUser).Take(20).ToList();
            var nearestIds = _infos.Select(i => i.GeofenceId).ToList();
            //stop monitoring geofences too far from user
            foreach (var monitoredRegion in _locationManager.MonitoredRegions) { 
                var monitoredGeofence = monitoredRegion as CLCircularRegion;
                if (monitoredGeofence != null) {
                    if (nearestIds.Contains(monitoredGeofence.Identifier))
                    {
                        nearestIds.Remove(monitoredGeofence.Identifier);
                    }
                    else { 
                        _locationManager.StopMonitoring(monitoredGeofence);
                    }
                }
            }
            //start monitoring the newest near geofences
            foreach (var geofenceId in nearestIds) {
                var geofence = _geofences.Find(g => g.Identifier.Equals(geofenceId));
                if (geofence != null)
                    _locationManager.StartMonitoring(geofence);
            }
        }
    }

    public class GeofenceInfo {
        public GeofenceInfo(string id) 
        {
            GeofenceId = id;
        }
        public string GeofenceId { get; private set; }
        public double DistanceFromUser { get; set;}
    }
}

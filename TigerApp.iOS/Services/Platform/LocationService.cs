using System;
using CoreLocation;
using System.Threading.Tasks;
using UIKit;
using System.Linq;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.Platform;

namespace TigerApp.iOS.Services.Platform
{
	public class LocationService : ILocationService
	{
		protected CLLocationManager LocationManager;
		private TaskCompletionSource<CLLocation> locationCompletionSource;

		public LocationService()
		{
			this.LocationManager = new CLLocationManager();
			if(UIDevice.CurrentDevice.CheckSystemVersion(8,0)) {
				LocationManager.DesiredAccuracy = CLLocation.AccuracyBest;
				LocationManager.DistanceFilter = CLLocationDistance.FilterNone;
				LocationManager.RequestWhenInUseAuthorization(); // works in foreground
			}
		}

		public bool IsLocationEnabled => CLLocationManager.LocationServicesEnabled &&
										(CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways ||
										 CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse);

		public async Task<Location> CurrentLocationAsync()
		{
			CLLocation location = await GetLocationAsync();
			Location point = new Location();

			point.Latitude = location.Coordinate.Latitude;
			point.Longitude = location.Coordinate.Longitude;

			return point;
		}

		private void OnLocationUpdated(object IChannelSender,CLLocationsUpdatedEventArgs e)
		{
			if(locationCompletionSource != null) {
				locationCompletionSource.SetResult(e.Locations.Last());
				locationCompletionSource = null;
			}

			LocationManager.StopUpdatingLocation();
			LocationManager.LocationsUpdated -= OnLocationUpdated;
		}

		private Task<CLLocation> GetLocationAsync()
		{
			locationCompletionSource = new TaskCompletionSource<CLLocation>();
			var lcsTask = locationCompletionSource.Task;

			try {
				if(IsLocationEnabled) {
					LocationManager.LocationsUpdated += OnLocationUpdated;
					LocationManager.StartUpdatingLocation();
				} else {
					locationCompletionSource.SetException(new GpsNotActiveException());
					locationCompletionSource = null;
				}
			} catch(Exception e) {
				locationCompletionSource.SetException(e);
				locationCompletionSource = null;
			}

			return lcsTask;
		}
	}
}
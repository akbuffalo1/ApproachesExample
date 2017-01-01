using System;
using System.Threading;
using System.Threading.Tasks;
using TigerApp.Shared.Services.Platform;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using AD.Plugins.CurrentActivity;

namespace TigerApp.Droid.Services.Platform
{
	public class LocationService: Java.Lang.Object, ILocationService, ILocationListener
	{
		private const long RefreshingFrequency = 1000;
		private const int GpsSearchingTimeOut = 8000;

		private LocationManager _locationManager;
		private Geocoder _geocoder;
		private ICurrentActivity _currentActivity;

		private bool wasLocationChanged = false;

        private TaskCompletionSource<TigerApp.Shared.Models.Location> geoPointTaskSource;

		public LocationService(ICurrentActivity currentActivity)
		{
			_currentActivity = currentActivity;
			InitializeLocationManager();
		}

		private void InitializeLocationManager()
		{
            var currentActivity = _currentActivity.Activity;

			_locationManager = (LocationManager)currentActivity.GetSystemService(Android.Content.Context.LocationService);
			_geocoder = new Geocoder(currentActivity,Java.Util.Locale.Default);
		}

		public bool IsLocationEnabled 
		{
			get 
			{
				return _locationManager.IsProviderEnabled(LocationManager.GpsProvider);
			}
		}

		public Task<TigerApp.Shared.Models.Location> CurrentLocationAsync()
		{
			geoPointTaskSource = new TaskCompletionSource<TigerApp.Shared.Models.Location>();

			StartGpsTimeOutToken();

			wasLocationChanged = false;
			RequestLocationChanges();

			return geoPointTaskSource.Task;
		}

		public void OnLocationChanged(Location location)
		{
			if(geoPointTaskSource != null && !wasLocationChanged) {
                geoPointTaskSource.SetResult(new TigerApp.Shared.Models.Location() { 
                    Latitude = location.Latitude, 
                    Longitude = location.Longitude
                });
				wasLocationChanged = true;
			}

			this.PauseLocationChanges();
		}

		public void OnProviderDisabled(string provider)
		{
			if(geoPointTaskSource != null && !wasLocationChanged) {
				geoPointTaskSource.SetException(new GpsNotActiveException());
				wasLocationChanged = true;
			}
		}

		public void OnProviderEnabled(string provider)
		{
		}

		public void OnStatusChanged(string provider,[GeneratedEnum] Availability status,Bundle extras)
		{
		}

		private void RequestLocationChanges()
		{
			try {
				if(TryToGetLastKnownLocation())
					return;

				_locationManager.RequestLocationUpdates(LocationManager.GpsProvider,RefreshingFrequency,0,this);
			} catch(Exception e) {
				if(geoPointTaskSource != null)
					geoPointTaskSource.SetException(e);
			}
		}

		private bool TryToGetLastKnownLocation()
		{
			Location location = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);

			if(location == null) {
				location = _locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
			}
			if(location != null) {
				geoPointTaskSource.SetResult(new TigerApp.Shared.Models.Location()
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                }); 
				wasLocationChanged = true;

				return true;
			}

			return false;
		}

		private void StartGpsTimeOutToken()
		{
			var cancelationToken = new CancellationTokenSource(GpsSearchingTimeOut);
			cancelationToken.Token.Register(StartNetworkTimeOutToken);
		}

		private void StartNetworkTimeOutToken()
		{
			var cancelationToken = new CancellationTokenSource(GpsSearchingTimeOut);
			cancelationToken.Token.Register(() => {
				var taskStatus = geoPointTaskSource.Task.Status;

				if(taskStatus == TaskStatus.WaitingForActivation || taskStatus == TaskStatus.WaitingToRun || taskStatus == TaskStatus.Running) {
					geoPointTaskSource.SetException(new GpsTimeoutException());
					wasLocationChanged = true;
				}

				PauseLocationChanges();
			});
		}

		private void PauseLocationChanges()
		{
			_locationManager.RemoveUpdates(this);
		}
	}
}
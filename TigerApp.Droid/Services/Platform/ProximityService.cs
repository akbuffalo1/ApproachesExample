using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using TigerApp.Shared.Models.TrackedActions;
using TigerApp.Shared.Services.Platform;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Gms.Common;
using TigerApp.Shared;
using Java.Lang;
using System.Linq;
using AD.Plugins.CurrentActivity;

namespace TigerApp.Droid.Services.Platform
{
    public class ProximityService: IGeofenceService
    {
        private const string TAG = "ProximityService";
        private List<IGeofence> _geofences;
        private List<Place> _places;
        protected GoogleApiClient _googleApiClient;
        private Action _onConnect = null;
        private Action<Place> _onEnter;
        private Action<Place> _onExit;

        public ProximityService() {
            GoogleApiServerHandler.OnConnect += (sender, e) => { 
                _onConnect?.Invoke();
                _onConnect = null;
            };
            GeofenceIntent.OnGeofencingEvent += (object sender, GeofencingEvent e) => {
                OnHandleGeofenceEvent(e);
            };
            _googleApiClient = GoogleApiServerHandler.Client;
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
                _geofences = new List<IGeofence>();
            if (_places == null)
                _places = new List<Place>();
            _geofences.Add(
                new GeofenceBuilder()
                .SetRequestId(place.PlaceId)
                    .SetCircularRegion(
                        place.Latitude,
                        place.Longitude,
                        (float)Constants.GeoFence.DefaultRadius
                    )
                .SetExpirationDuration(Constants.GeoFence.ExpirationInMilliseconds)
                .SetTransitionTypes(Geofence.GeofenceTransitionEnter |
                        Geofence.GeofenceTransitionExit)
                .Build()
            );
            _places.Add(place);
        }

        public void SaveAll()
        {
        }

        public bool StartMonitoring(Action<Place> onEnter = null, Action<Place> onExit = null)
        {
            try
            {
                _onEnter = onEnter;
                _onExit = onExit;
                if (_googleApiClient.IsConnected)
                    _addGeofenceRequest();
                else {
                    _onConnect = _addGeofenceRequest;
                    _googleApiClient.Connect();
                }
            }catch(System.Exception ex){
                Console.WriteLine(string.Format("[{0}] : {1}",TAG,ex.Message));
                return false;
            }
            return true;
        }

        public bool StopMonitoring()
        {
            try
            {
                _onEnter = null;
                _onExit = null;
                if (_googleApiClient.IsConnected)
                    _removeGeofenceRequest();
                else {
                    _onConnect = _removeGeofenceRequest;
                    _googleApiClient.Connect();
                }
            }
            catch (System.Exception ex){
                Console.WriteLine(string.Format("[{0}] : {1}", TAG, ex.Message));
                return false;
            }
            return true;
        }

        protected void OnHandleGeofenceEvent(GeofencingEvent geofencingEvent)
        {
            if (geofencingEvent.HasError)
            {
                Console.WriteLine("{0} : Geofence monitoring failed with error {1}", TAG, geofencingEvent.ErrorCode);
                return;
            }

            Console.WriteLine("{0} : Geofence event fire!!!", TAG);
            int geofenceTransition = geofencingEvent.GeofenceTransition;

            if (geofenceTransition == Geofence.GeofenceTransitionEnter)
            {
                var triggeringPlaces = _getPlacesFromEvent(geofencingEvent);
                foreach (var place in triggeringPlaces)
                    _onEnter?.Invoke(place);
            }
            else if (geofenceTransition == Geofence.GeofenceTransitionExit) { 
                var triggeringPlaces = _getPlacesFromEvent(geofencingEvent);
                foreach (var place in triggeringPlaces)
                    _onExit?.Invoke(place);
            }
            else {
                Console.WriteLine(string.Format("[{0}] : Unkown transition! ", TAG));
            }
        }

        private GeofencingRequest _getGeofencingRequest()
        {
            var builder = new GeofencingRequest.Builder();
            builder.SetInitialTrigger(GeofencingRequest.InitialTriggerEnter);
            builder.AddGeofences(_geofences);

            return builder.Build();
        }

        private async void _addGeofenceRequest()
        {
          
            try
            {
                var status = await LocationServices.GeofencingApi.AddGeofencesAsync(_googleApiClient, _getGeofencingRequest(),
                                                                                    GoogleApiServerHandler.GeofencePendingIntent);
                _handleConnectionResult(status);
            }
            catch (SecurityException securityException)
            {
                Console.WriteLine(string.Format("{0} : Invalid location permission. You need to use ACCESS_FINE_LOCATION with geofences. {1}", TAG, securityException.Message));
            }
        }

        private async void _removeGeofenceRequest()
        {
            try
            {
                var status = await LocationServices.GeofencingApi.RemoveGeofencesAsync(_googleApiClient,
                                                                                       GoogleApiServerHandler.GeofencePendingIntent);
                _handleConnectionResult(status);
            }
            catch (SecurityException securityException)
            {
                Console.WriteLine(string.Format("{0} : Invalid location permission. You need to use ACCESS_FINE_LOCATION with geofences. {1}", TAG, securityException.Message));
            }
        }

        private void _handleConnectionResult(Statuses status) { 

            if (status.IsSuccess)
            {
                Console.WriteLine(string.Format("{0} : Geofences successfully added!", TAG));
            }
            else {
                Console.WriteLine(string.Format("{0} : Failed to add geofences! [error code : {1}] : {2}", TAG, status.StatusCode, status.StatusMessage));
            }
        }

        private List<Place> _getPlacesFromEvent(GeofencingEvent geofencingEvent) { 
            var geofenceIds = geofencingEvent.TriggeringGeofences.Select(geofence => geofence.RequestId).ToList();
            return _places.FindAll(place => geofenceIds.Contains(place.PlaceId)).ToList();
        } 
    }

    [Service]
    public class GeofenceIntent : IntentService
    {
        public static event EventHandler<GeofencingEvent> OnGeofencingEvent;

        protected override void OnHandleIntent(Intent intent)
        {
            #if DEBUG
            //////////////TODO REMOVE
            /*var actIntent = new Intent(this, typeof(Pages.ExpHomeActivity));
            actIntent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, actIntent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetContentTitle("Ciao! entra in Tiger e aumenta i tuoi punti!")
                .SetContentText("Ciao! entra in Tiger e aumenta i tuoi punti!")
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());*/
            ////////////
            #endif

            var geofencingEvent = GeofencingEvent.FromIntent(intent);
            OnGeofencingEvent?.Invoke(this,geofencingEvent);
        }
    }

    public class GoogleApiConnectionCallbacks : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private const string TAG = "GoogleApiConnectionCallback";
        public event EventHandler OnConnection;

        public void OnConnected(Bundle connectionHint)
        {
            Console.WriteLine("{0} : Connected to GoogleApiService", TAG);
            OnConnection?.Invoke(this,null);
        }

        public void OnConnectionSuspended(int cause)
        {
            Console.WriteLine("{0} : Connection to GoogleApiService suspended", TAG);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Console.WriteLine("{0} : Connection to GoogleApiService failed with error [{1}] {2} ", TAG, result.ErrorCode, result.ErrorMessage);
        }
    }
}

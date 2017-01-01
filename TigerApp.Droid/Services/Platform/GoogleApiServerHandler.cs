using System;
using Android.Gms.Common.Apis;
using Android.Content;
using Android.Gms.Location;
using Android.App;

namespace TigerApp.Droid.Services.Platform
{
    public class GoogleApiServerHandler
    {
        private static GoogleApiConnectionCallbacks _googleApiConnectionCallbacks;
        private static GoogleApiClient _googleApiClient;
        public static GoogleApiClient Client { 
            get { 
                return _googleApiClient;
            }
        }
        public static event EventHandler OnConnect;

        private static PendingIntent _geofencingPendingIntent;
        public static PendingIntent GeofencePendingIntent
        {
            get {
                return _geofencingPendingIntent;
            }
        }

        public static void InitServer(Context ctx)
        {
            _googleApiConnectionCallbacks = new GoogleApiConnectionCallbacks();
            _googleApiConnectionCallbacks.OnConnection += (sender, e) => OnConnect?.Invoke(sender,e);
            _googleApiClient = new GoogleApiClient.Builder(ctx)
                .AddConnectionCallbacks(_googleApiConnectionCallbacks)
                .AddOnConnectionFailedListener(_googleApiConnectionCallbacks)
                .AddApi(LocationServices.API)
                .Build();
            if (_geofencingPendingIntent == null)
            {
                var intent = new Intent(ctx, typeof(GeofenceIntent));
                _geofencingPendingIntent = PendingIntent.GetService(ctx, 0, intent, PendingIntentFlags.UpdateCurrent);
            }

        }
    }
}

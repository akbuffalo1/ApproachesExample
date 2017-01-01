using System;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using TigerApp.Shared.Services.API;
using AD;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Android.OS;
using TigerApp.Shared;

namespace TigerApp.Droid.Services.Platform
{
    [Service]
    public class RegistrationIntentService : IntentService
    {
        private const string SENDER_ID = "313597449559";
        public const string INTENT_MESSAGE = "com.byters.TigerApp.notifications.REQUEST_INTENT_MESSAGE";
        public const string INTENT_DATA = "INTENT_DATA";
        private const string LOG_TAG = nameof(RegistrationIntentService);

        static object locker = new object();
        private ILogger _logger;
        private IFlagStoreService _flagStore;

        public RegistrationIntentService() : base("RegistrationIntentService") 
        {
            _logger = Resolver.Resolve<ILogger>();
            _flagStore = Resolver.Resolve<IFlagStoreService>();
        }

        protected override void OnHandleIntent(Intent intent)
        {
            _logger.Debug(LOG_TAG, "Calling InstanceID.GetToken");
            lock (locker)
            {
                ObtainTokenObservable().Subscribe(
                    token =>
                    {
                        _logger.Debug(LOG_TAG, "GCM Registration Token: " + token);    
                        var tokenCompletionIntent = new Intent(INTENT_MESSAGE);
                        var bundle = new Bundle();
                        bundle.PutString(INTENT_DATA, token);
                        tokenCompletionIntent.PutExtras(bundle);
                        SendBroadcast(tokenCompletionIntent);
                    }, error =>
                    {
                        _logger.Debug(LOG_TAG, error.Message);
                        _flagStore.Unset(Constants.Flags.ASKED_FOR_NOTIFICATIONS);
                    });
            }
        }

        public IObservable<string> ObtainTokenObservable()
        {
            return Observable.Create<string>(async obs =>
            {
                try
                {
                    var instanceID = InstanceID.GetInstance(this);

                    var token = await instanceID.GetTokenAsync(SENDER_ID, GoogleCloudMessaging.InstanceIdScope, null);

                    if (!string.IsNullOrEmpty(token))
                    {
                        obs.OnNext(token);
                        obs.OnCompleted();
                        GcmPubSub.GetInstance(this).Subscribe(token, "/topics/global", null);
                        //Intent registrationComplete = new Intent("registationComplete");
                        //SendBroadcast(registrationComplete);
                    }
                    else
                    {
                        obs.OnError(new TokenException());
                    }
                }
                catch (Exception e)
                {
                    obs.OnError(e);
                }
                return Disposable.Empty;
            }).ObserveOnUI();
        }
    }

    public class TokenException : Exception {
        public TokenException() : base("Token has not been obtained") { }
    }
}
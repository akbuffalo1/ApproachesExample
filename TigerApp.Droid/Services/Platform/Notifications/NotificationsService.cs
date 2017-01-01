using System;
using AD.Plugins.Permissions;
using ReactiveUI;
using System.Reactive.Linq;
using TigerApp.Shared.Services.API;
using TigerApp.Shared;
using ReactiveUI.Fody.Helpers;
using Android.Content;
using AD.Plugins.CurrentActivity;
using TigerApp.Droid.Utils;
using Android.App;
using AD;

namespace TigerApp.Droid.Services.Platform
{
    public class NotificationsService : ReactiveObject, INotificationsService
    {
        public IReactiveCommand RequestPermissions
        {
            get;
            protected set;
        }

        [Reactive]
        public PermissionStatus PermissionStatus
        {
            get;
            protected set;
        } = PermissionStatus.Unknown;

        private TokenReceiver _receiver;
        private readonly IFlagStoreService _flagStore;
        private readonly ICurrentActivity _current;
        private readonly INotificationsApiService _notificationApiService;

        public NotificationsService(INotificationsApiService notificationApiService, IFlagStoreService flagStoreService, ICurrentActivity current)
        {
            _notificationApiService = notificationApiService;
            _flagStore = flagStoreService;
            _current = current;

            _flagStore.ExecuteIfSetOrNot(Constants.Flags.ASKED_FOR_NOTIFICATIONS,
                actIfYes:() =>
                {
                    PermissionStatus = PermissionStatus.Granted;
                },
                actIfNot: () =>
                {
                    RequestPermissions = ReactiveCommand.CreateAsyncObservable<bool>(o =>
                    {
                        if (PlatformUtil.IsGooglePlayServicesAvailable(_current.Activity))
                        {
                            var intent = new Intent(_current.Activity, typeof(RegistrationIntentService));
                            _current.Activity.StartService(intent);
                        }
                        else
                        {
                            PermissionStatus = PermissionStatus.Denied;
                        }
                        return Observable.Return(true);
                    });
                });
        }

        public void RegisterReceiver(Activity activity)
        {
            _receiver = new TokenReceiver();
            _receiver.OnTokenReceived += HandleToken;
            IntentFilter intentFilter = new IntentFilter(RegistrationIntentService.INTENT_MESSAGE);
            intentFilter.AddCategory(Intent.CategoryDefault);
            activity.RegisterReceiver(_receiver, intentFilter);
        }

        public void UnRegisterReceiver(Activity activity)
        {
            _receiver.OnTokenReceived -= HandleToken;
            activity.UnregisterReceiver(_receiver);
            _receiver = null;
        }

        private void HandleToken(string registrationToken)
        {
            var payload = new System.Collections.Generic.List<Shared.Models.ServerAction>
            {
                new Shared.Models.ServerAction {
                    Action = "replace",
                    Path = "/registration_id",
                    Value = registrationToken
                }
            };
            _notificationApiService.SetNotificationsEnabled(payload)
                .Catch(error =>
                {
                    PermissionStatus = PermissionStatus.Denied;
                }).SubscribeOnce((obj) =>
                {
                    PermissionStatus = PermissionStatus.Granted;
                    _flagStore.Set(Constants.Flags.ASKED_FOR_NOTIFICATIONS);
                });
        }
    }

    [BroadcastReceiver]
    [IntentFilter(new[] { RegistrationIntentService.INTENT_MESSAGE })]
    public class TokenReceiver : BroadcastReceiver
    {
        public delegate void TokenReceived(string token);
        public event TokenReceived OnTokenReceived;

        public override void OnReceive(Context context, Intent intent)
        {
            if (!string.IsNullOrEmpty(intent?.GetStringExtra(RegistrationIntentService.INTENT_DATA)))
            {
                var handler = OnTokenReceived;
                if (handler != null)
                    handler(intent.GetStringExtra(RegistrationIntentService.INTENT_DATA));
                else
                    Resolver.Resolve<IFlagStoreService>().Unset(Constants.Flags.ASKED_FOR_NOTIFICATIONS);
            }
        }
    }
}
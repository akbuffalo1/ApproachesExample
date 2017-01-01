using System;
using UIKit;
using System.Reactive.Linq;

using Foundation;
using ReactiveUI;
using AD.Plugins.Permissions;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared;

namespace TigerApp.iOS.Services.Platform
{
    public class NotificationsService : ReactiveObject, INotificationsService
    {
        private GCMReceiverHandler _gcmReceiveHandler;
        private readonly UIUserNotificationType LocalNotificationTypes = UIUserNotificationType.Badge | UIUserNotificationType.Alert;
        private readonly UIRemoteNotificationType RemoteNotificationTypes = UIRemoteNotificationType.Badge | UIRemoteNotificationType.Alert;
        private static Action _onConnect;
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

        public bool AreNotificationsEnabled => UIApplication.SharedApplication.CurrentUserNotificationSettings.Types != UIUserNotificationType.None;

        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var config = Google.InstanceID.Config.DefaultConfig;
            Google.InstanceID.InstanceId.SharedInstance.Start(config);
            _gcmReceiveHandler.DeviceKey = deviceToken;
        }

        private void UpdatePermissionStatus()
        {
            bool granted = AreNotificationsEnabled;

            var askedForNotifications = FlagStore.IsSet(Constants.Flags.ASKED_FOR_NOTIFICATIONS);

            if (granted)
            {
                PermissionStatus = PermissionStatus.Granted;
            }
            else {
                if (askedForNotifications)
                {
                    PermissionStatus = PermissionStatus.Denied;
                }
                else {
                    PermissionStatus = PermissionStatus.Unknown;
                }
            }
        }

        public void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            FlagStore.Set(Constants.Flags.ASKED_FOR_NOTIFICATIONS);
            UpdatePermissionStatus();
        }

        public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Google.GoogleCloudMessaging.Service.SharedInstance.AppDidReceiveMessage(userInfo);
        }

        public static void Connect() {
            Google.GoogleCloudMessaging.Service.SharedInstance.Connect((NSError err) => {
                if (err != null)
                    Console.WriteLine(err.LocalizedDescription);
                else if (_onConnect != null)
                    _onConnect();
            });
        }

        public static void Disconnect() {
            Google.GoogleCloudMessaging.Service.SharedInstance.Disconnect();
        }

        private NSMutableSet NotificationCategories = new NSMutableSet<UIUserNotificationCategory>();

        private readonly IFlagStoreService FlagStore;

        public NotificationsService(IFlagStoreService flagStoreService)
        {
            FlagStore = flagStoreService;

            UpdatePermissionStatus();

            RequestPermissions = ReactiveCommand.CreateAsyncObservable<bool>(o =>
            {
                if (!AreNotificationsEnabled)
                {
                    /* TODO: Setup notifications types  */
                }
                else {
                    return Observable.Return(false);
                }

                // Configure and Start GCM
                _gcmReceiveHandler = new GCMReceiverHandler();
                _onConnect = () => { _gcmReceiveHandler.SubscribeForTopic();};
                _gcmReceiveHandler.OnRegistrationTokenObtained += (sender, registrationToken) => {
                    var payload = new System.Collections.Generic.List<Shared.Models.ServerAction>(){
                    new Shared.Models.ServerAction {
                            Action = "replace",
                            Path = "/registration_id",
                            Value = registrationToken
                        }
                    };
                    AD.Resolver.Resolve<AD.IApiClient>().MakeRequestFor<Shared.Services.Platform.RegisterForPushNotificationResponseEntity, System.Collections.Generic.List<Shared.Models.ServerAction>>(
                        "/api/v1/device",
                        payload,
                        (obj) =>
                        {
                            Console.WriteLine(obj);
                        },
                        (err) =>
                        {
                            Console.WriteLine(err.Message);
                        },
                        "PATCH"
                    );
                };

                var gcmConfig = Google.GoogleCloudMessaging.Config.DefaultConfig;
                gcmConfig.ReceiverDelegate = _gcmReceiveHandler;
                Google.GoogleCloudMessaging.Service.SharedInstance.Start(gcmConfig);

                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(LocalNotificationTypes, NotificationCategories);
                    UIApplication.SharedApplication.RegisterUserNotificationSettings(notificationSettings);
                    UIApplication.SharedApplication.RegisterForRemoteNotifications();
                }
                else {
                    UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                    UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
                }
                return Observable.Return(AreNotificationsEnabled);
            });
        }
    }

    public class GCMReceiverHandler : NSObject, Google.GoogleCloudMessaging.IReceiverDelegate
    {
        private NSData _deviceKey;
        private string _deviceToken;
        private Google.Core.Configuration _configuration;
        public event EventHandler<string> OnRegistrationTokenObtained;

        public NSData DeviceKey { 
            get {
                return _deviceKey;
            }
            set {
                _deviceKey = value;
                _getToken();
            }
        }

        public GCMReceiverHandler() {
            // Configure our core Google 
            NSError err;
            Google.Core.Context.SharedInstance.Configure(out err);
            if (err != null)
                Console.WriteLine("Failed to configure Google: {0}", err.LocalizedDescription);
            _configuration = Google.Core.Context.SharedInstance.Configuration;
        }

        private void _getToken()
        {
            // Register APNS Token to GCM
            var options = new NSMutableDictionary();
            options.SetValueForKey(DeviceKey, Google.InstanceID.Constants.RegisterAPNSOption);
            #if DEBUG
            options.SetValueForKey(new NSNumber(true), Google.InstanceID.Constants.APNSServerTypeSandboxOption);
            #else
            options.SetValueForKey(new NSNumber(false), Google.InstanceID.Constants.APNSServerTypeSandboxOption);
            #endif

            // Get our token
            Google.InstanceID.InstanceId.SharedInstance.Token(
                _configuration.GcmSenderId,
                Google.InstanceID.Constants.ScopeGCM,
                options,
                (token, error) => {
                    NSMutableDictionary userInfo = new NSMutableDictionary();
                    if (error == null)
                    {
                        _deviceToken = token;
                        userInfo.SetValueForKey(new NSString(token), new NSString("registrationToken"));
                        if (OnRegistrationTokenObtained != null)
                            OnRegistrationTokenObtained(this, _deviceToken);
                    }else{
                        userInfo.SetValueForKey(new NSString(error.LocalizedDescription), new NSString("error"));
                    }
                    NSNotificationCenter.DefaultCenter.PostNotificationName("onRegistationCompleted", null, userInfo);
                    SubscribeForTopic();
            });
        }

        public string DeviceToken { 
            get {
                return _deviceToken;
            }
            set {
                _deviceToken = value;
                if (OnRegistrationTokenObtained != null)
                    OnRegistrationTokenObtained(this,value);
            }
        }

        public void SubscribeForTopic() {
            if (_deviceToken == null)
                return;
            Google.GoogleCloudMessaging.PubSub.SharedInstance.Subscribe(_deviceToken, "/topics/global", new NSDictionary(), new Google.GoogleCloudMessaging.PubSubCompletion(_handleSubscriptionError));
        }

        private void _handleSubscriptionError(NSError error) {
            if(error != null)
                Console.WriteLine(String.Format("Subscription Failed! : {0}",error.LocalizedDescription));
        }

    }
}

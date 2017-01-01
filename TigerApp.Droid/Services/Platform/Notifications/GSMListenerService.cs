using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using System;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Models.PushNotifications;

namespace TigerApp.Droid.Services.Platform.Notifications
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class GSMListenerService : GcmListenerService
    {
        public override void OnMessageReceived(string from, Bundle data)
        {
            foreach(var key in data.KeySet())
                Console.WriteLine(string.Format("######{0} : {1}",key,data.Get(key)));
            var notification = PushNotificationsFactory.CreateFromSource(new DroidPushNotifcationsSource(data));

            var currentActivity = AD.Resolver.Resolve<AD.Plugins.CurrentActivity.ICurrentActivity>().Activity;
            if (currentActivity.GetForegroundState())
                AD.Resolver.Resolve<AD.Plugins.CurrentActivity.ICurrentActivity>().Activity.HandleNotification(notification);
            else
                SendNotification(notification);
        }

        // Use Notification Builder to create and launch the notification:
        void SendNotification(GCMPushNotification notification)
        {
            var pendingIntent = _getRestartActivityIntentFromNotification(notification);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetContentTitle("TigerApp")
                .SetContentText(notification.Message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

        private PendingIntent _getRestartActivityIntentFromNotification(GCMPushNotification notification) { 
            var current = AD.Resolver.Resolve<AD.Plugins.CurrentActivity.ICurrentActivity>().Activity;
            var type = current != null ? current.GetType() : typeof(Pages.ExpHomeActivity);
            var pointsNotification = (notification as PointsAddedPushNotifications);

            if (pointsNotification != null)
            {
                if (pointsNotification.IsMission)
                    type = typeof(Pages.MissionList.MissionsListActivity);
                else
                    type = typeof(Pages.ExpHomeActivity);
            }
            else{
                var isLevelUpNotification = (notification as LevelUpPushNotification) != null;
                if (isLevelUpNotification)
                    type = typeof(Pages.WallOfAvatarActivity);
                else if ((notification as CheckInSurveyPushNotification) != null)
                    type = typeof(Pages.CheckInSurveyActivity);
            }
            var intent = new Intent(this, type);
            intent.AddFlags(ActivityFlags.ClearTop);
            if(type.Equals(typeof(Pages.CheckInSurveyActivity)))
                intent.PutExtra(Pages.CheckInSurveyActivity.StoreIdIntentKey,(notification as CheckInSurveyPushNotification).StoreId);
            return PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
        }

    }
}    
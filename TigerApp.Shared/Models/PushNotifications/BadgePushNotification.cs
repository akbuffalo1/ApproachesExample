using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public class BadgePushNotification : TigerPushNotification
    {
        public const string BadgeNameKey = "badge_name";
        public BadgePushNotification(IPushNotificationSource source):base(source)
        {
            BadgeName = source.Get(BadgeNameKey);
        }

        public string BadgeName 
        {
            get;
            protected set;
        }
    }
}

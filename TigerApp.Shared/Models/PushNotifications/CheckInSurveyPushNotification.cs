using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public class CheckInSurveyPushNotification : TigerPushNotification
    {
        public CheckInSurveyPushNotification(IPushNotificationSource source) : base(source)
        {
            StoreId = source.Get(StoreIdKey);
        }

        public const string StoreIdKey = "store_id";

        public string StoreId
        {
            get;
            protected set;
        }
    }
}

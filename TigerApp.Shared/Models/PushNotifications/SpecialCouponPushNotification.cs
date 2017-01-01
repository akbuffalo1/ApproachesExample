using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public class SpecialCouponPushNotification : CouponPushNotification
    {
        public SpecialCouponPushNotification(IPushNotificationSource source):base(source)
        {
        }
    }
}

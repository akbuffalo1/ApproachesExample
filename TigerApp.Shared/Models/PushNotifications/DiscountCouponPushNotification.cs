using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public class DiscountCouponPushNotification : CouponPushNotification
    {
        public DiscountCouponPushNotification(IPushNotificationSource source):base(source)
        {
            Amount = source.Get(AmountKey);
        }

        public const string AmountKey = "amount";

        public string Amount 
        {
            get;
            protected set;
        }
    }
}

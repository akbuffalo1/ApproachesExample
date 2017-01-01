using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public class PushNotificationsFactory
    {
        public static GCMPushNotification CreateFromSource(IPushNotificationSource source)
        {
            var typeString = source.Get(TigerPushNotification.TypeKey);
            if (!string.IsNullOrEmpty(typeString)) {    
                var points = source.Get(PointsAddedPushNotifications.PointsKey);
                if (!string.IsNullOrEmpty(points))
                    return _createPointsAddedPushNotificationFromSource(source);
                else {
                    var userLevel = source.Get(LevelUpPushNotification.UserLevelKey);
                    if (!string.IsNullOrEmpty(userLevel))
                        return _createLevelUpPushNotificationFromSource(source);
                    else {
                        var badgeName = source.Get(BadgePushNotification.BadgeNameKey);
                        if (!string.IsNullOrEmpty(badgeName))
                            return _createBadgeNotificationFromSource(source);
                        else {
                            var type = source.Get(TigerPushNotification.TypeKey);
                            if(type.EndsWith("coupon",StringComparison.CurrentCulture))
                                return _createCouponPushNotificationFromSource(source);
                            else { 
                                var storeId = source.Get(CheckInSurveyPushNotification.StoreIdKey);
                                if (!string.IsNullOrEmpty(storeId))
                                    return _createCheckinSurveyPushNotificationFromSource(source);
                                return new TigerPushNotification(source);
                            }
                        }
                    }
                }
            }
             
            return new GCMPushNotification(source);
        }

        private static PointsAddedPushNotifications _createPointsAddedPushNotificationFromSource(IPushNotificationSource source)
        {
            return new PointsAddedPushNotifications(source);
        }

        private static LevelUpPushNotification _createLevelUpPushNotificationFromSource(IPushNotificationSource source)
        {
            return new LevelUpPushNotification(source);
        }

        private static BadgePushNotification _createBadgeNotificationFromSource(IPushNotificationSource source)
        {
            return new BadgePushNotification(source);
        }

        private static CouponPushNotification _createCouponPushNotificationFromSource(IPushNotificationSource source)
        {
            var amount = source.Get(DiscountCouponPushNotification.AmountKey);
            if (!string.IsNullOrEmpty(amount))
                return new DiscountCouponPushNotification(source);
            else
                return new SpecialCouponPushNotification(source);
        }

        private static CheckInSurveyPushNotification _createCheckinSurveyPushNotificationFromSource(IPushNotificationSource source)
        {
            return new CheckInSurveyPushNotification(source);
        }
    }
}

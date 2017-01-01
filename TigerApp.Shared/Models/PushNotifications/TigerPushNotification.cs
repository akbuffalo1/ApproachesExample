using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public enum TigerNotificationsType { 
        Unknwon,Points,Mission,LevelUp,Badge,Coupon,FirstMission,NewProducts,CheckInSurvey,Geofence
    }
    public class TigerPushNotification:GCMPushNotification
    {
        public const string NickNameKey = "nickname";
        public const string TypeKey = "notification_type";
        public TigerPushNotification(IPushNotificationSource source) : base(source)
        {
            NickName = source.Get(NickNameKey);
            _type = source.Get(TypeKey);
        }

        public string NickName
        {
            get;
            protected set;
        }

        private string _type;

        public TigerNotificationsType NotificationType{
            get{
                switch (_type) {
                    case "geofence-2":
                        return TigerNotificationsType.CheckInSurvey;
                    case "level-up":
                        return TigerNotificationsType.LevelUp;
                    case "scan-receipt":
                        return TigerNotificationsType.Points;
                    case "mission-completed":
                        return TigerNotificationsType.Mission;
                    case "first-mission":
                        return TigerNotificationsType.FirstMission;
                    case "new-products":
                        return TigerNotificationsType.NewProducts;
                    case "geofence-1":
                        return TigerNotificationsType.Geofence;
                    case "get-badge":
                        return TigerNotificationsType.Badge;
                    case "level-up-special-coupon":
                        return TigerNotificationsType.Coupon;
                    case "discount-coupon":
                        return TigerNotificationsType.Coupon;
                }
                return TigerNotificationsType.Unknwon;
            }
        }
    }
}

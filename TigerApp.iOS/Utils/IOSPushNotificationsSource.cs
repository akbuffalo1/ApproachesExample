using System;
using Foundation;
using TigerApp.Shared.Models.PushNotifications;

namespace TigerApp.iOS.Utils
{
    public class IOSPushNotificationsSource:IPushNotificationSource
    {
        private NSDictionary _data;

        public IOSPushNotificationsSource(NSDictionary data)
        {
            _data = data;
        }

        public string Get(string key)
        {
            var nsKey = new NSString(key);
            if (!_data.ContainsKey(nsKey))
                return null;
            return _data[nsKey].ToString();
        }

        public IPushNotificationSource GetSource(string key)
        {
            var nsKey = new NSString(key);
            return new IOSPushNotificationsSource(_data[nsKey] as NSDictionary);
        }
    }
}

using System;
using TigerApp.Shared.Models.PushNotifications;
using Android.OS;

namespace TigerApp.Droid.Utils
{
    public class DroidPushNotifcationsSource : IPushNotificationSource
    {
        private Bundle _data;

        public DroidPushNotifcationsSource(Bundle data)
        {
            /*if(data.GetString("message", "").EndsWith("Se aspetti il prossimo è più che raddoppiato!"))
                foreach (var key in data.KeySet())
                    Console.WriteLine(string.Format("@@@@@@@@@@@@@ {0} : {1}", key, data.Get(key)));*/
            _data = data;
        }

        public string Get(string key)
        {
            if (_data == null)
                return null;
            return _data.GetString(key, null);
        }

        public IPushNotificationSource GetSource(string key) 
        {
            if (_data == null)
                return null;
            return new DroidPushNotifcationsSource(_data.Get(key) as Bundle);
        }
    }
}

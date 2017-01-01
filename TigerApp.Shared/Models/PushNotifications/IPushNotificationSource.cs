using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public interface IPushNotificationSource
    {
        string Get(string key);
        IPushNotificationSource GetSource(string key);
    }
}

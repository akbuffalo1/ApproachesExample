using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public class GCMNotificationBody
    {
        public const string IconKey = "icon";
        public const string BodyKey = "body";
        public const string TitleKey = "title";
        public const string SoundKey = "sound";

        public string Icon 
        { 
            get; 
            protected set; 
        }

        public string Body 
        { 
            get; 
            protected set; 
        }

        public string Title 
        { 
            get; 
            protected set; 
        }

        public string Sound 
        { 
            get; 
            protected set; 
        }

        public GCMNotificationBody(IPushNotificationSource source) {
            Icon = source.Get(IconKey);
            Body = source.Get(BodyKey);
            Title = source.Get(TitleKey);
            Sound = source.Get(SoundKey);
        }
    }

    public class GCMPushNotification
    {
        public const string MessageIDKey = "google.message_id";
        public const string ApnsMessageIDKey = "gcm.message_id";
        public const string MessageKey = "message";
        public const string TitleKey = "title";
        public const string NotificationKey = "notification";
        public const string AlertKey = "alert";
        public const string ApnsKey = "aps";

        public static string NoMessageIDExceptionMessage = string.Format("Unable to find {0} in notifcation json!",MessageIDKey);

        public GCMPushNotification(IPushNotificationSource source) {

            bool fromApns = source.Get(ApnsKey) != null;
            MessageID = source.Get(fromApns ? ApnsMessageIDKey : MessageIDKey);
            if (MessageID == null)
                throw new ArgumentException(NoMessageIDExceptionMessage);
            Title = source.Get(TitleKey);
            Message = source.Get(MessageKey);
            var notificationSource = source.GetSource(fromApns ? ApnsKey : NotificationKey);
            if (fromApns)
                notificationSource = notificationSource.GetSource(AlertKey);
            Notification = new GCMNotificationBody(notificationSource);
        }

        public string Message
        {
            get;
            protected set;
        }

        public string Title
        {
            get;
            protected set;
        }

        public GCMNotificationBody Notification
        {
            get;
            protected set;
        }

        public string MessageID
        {
            get;
            protected set;
        }

        public override bool Equals(object obj)
        {
            return this.MessageID.Equals((obj as GCMPushNotification)?.MessageID,StringComparison.CurrentCulture);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public class LevelName
    {
        public const string NameKey = "name_of_new_level";
        public const string PluralKey = "name_of_new_level_plural";

        public string Name { get; set; }
        public string Plural { get; set; }

        public LevelName() { }

        public LevelName(IPushNotificationSource source) {
            Name = source.Get(NameKey);
            Plural = source.Get(PluralKey);
        }
    }

    public class LevelUpPushNotification:TigerPushNotification
    {
        public const string UserLevelKey = "user_level";
        public const string UserLevelOrdinalKey = "user_level_ordinal";
        public const string PrevUserLevelOrdinalKey = "prev_user_level_ordinal";

        public LevelUpPushNotification(IPushNotificationSource source) : base(source) 
        {
            UserLevel = source.Get(UserLevelKey);
            UserLevelOrdinal = source.Get(UserLevelOrdinalKey);
            PrevUserLevelOrdinal = source.Get(PrevUserLevelOrdinalKey);
            NewLevelName = new LevelName(source);
        }

        public string UserLevel
        {
            get;
            protected set;
        }

        public string UserLevelOrdinal
        {
            get;
            protected set;
        }

        public string PrevUserLevelOrdinal
        {
            get;
            protected set;
        }

        public LevelName NewLevelName
        {
            get;
            protected set;
        }
    }
}

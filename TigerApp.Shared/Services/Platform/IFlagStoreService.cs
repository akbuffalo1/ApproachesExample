using System;

namespace TigerApp
{
    public interface IFlagStoreService
    {
        void Set(string key);
        void Unset(string key);
        bool IsSet(string key);
        void UnsetAll();
    }

    public static class IFlagStoreEx
    {
        public static void ExecuteIfSet(this IFlagStoreService self, string key, Action act)
        {
            if (self.IsSet(key))
            {
                act();
            }
        }

        public static void ExecuteIfNotSet(this IFlagStoreService self, string key, Action act)
        {
            if (!self.IsSet(key))
            {
                act();
            }
        }

        public static void ExecuteIfSetOrNot(this IFlagStoreService self, string key, Action actIfYes, Action actIfNot)
        {
            if (self.IsSet(key))
                actIfYes();
            else
                actIfNot();
        }

        public static void ExecuteIfSetThenUnset(this IFlagStoreService self, string key, Action act)
        {
            if (self.IsSet(key))
            {
                act();
                self.Unset(key);
            }
        }

        public static void ExecuteIfNotSetThenSet(this IFlagStoreService self, string key, Action act)
        {
            if (!self.IsSet(key))
            {
                act();
                self.Set(key);
            }
        }
    }
}
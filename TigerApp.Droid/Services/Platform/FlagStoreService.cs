using Android.Content;

namespace TigerApp.Droid.Services.Platform
{
    public class FlagStoreService : IFlagStoreService
    {
        private const string FlagStorageName = "TigelFlagSore";

        public void Set(string key)
        {
            var act = AD.Resolver.Resolve<AD.Plugins.CurrentActivity.ICurrentActivity>().Activity;
            act.GetSharedPreferences(FlagStorageName, FileCreationMode.Private).Edit().PutBoolean(key, true).Apply();
        }

        public void Unset(string key)
        {
            var act = AD.Resolver.Resolve<AD.Plugins.CurrentActivity.ICurrentActivity>().Activity;
            act.GetSharedPreferences(FlagStorageName, FileCreationMode.Private).Edit().PutBoolean(key, false).Apply();
        }

        public bool IsSet(string key)
        {
            var act = AD.Resolver.Resolve<AD.Plugins.CurrentActivity.ICurrentActivity>().Activity;
            return act.GetSharedPreferences(FlagStorageName, FileCreationMode.Private).GetBoolean(key, false);
        }

        public void UnsetAll()
        {
            var act = AD.Resolver.Resolve<AD.Plugins.CurrentActivity.ICurrentActivity>().Activity;
            var preferences = act.GetSharedPreferences(FlagStorageName, FileCreationMode.Private);
            var  editor = preferences.Edit();
            editor.Clear();
            editor.Commit();
        }
    }
}
using Foundation;

namespace TigerApp.iOS.Services.Platform
{
    public class FlagStoreService : IFlagStoreService
    {
        public bool IsSet(string key)
        {
            return NSUserDefaults.StandardUserDefaults.BoolForKey(key);
        }

        public void UnsetAll()
        {
            throw new System.NotImplementedException();
        }

        public void Set(string key)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(true, key);
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public void Unset(string key)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(false, key);
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }
    }
}

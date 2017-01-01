using System;
using Foundation;
using TigerApp.Shared.Services.API;
namespace TigerApp.iOS.Configs
{
    public class IOSApiVersionConfig : BaseApiVersionConfig
    {
        public override string AppVersion
        {
            get
            {
                return NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
            }
            protected set
            {
                base.AppVersion = value;
            }
        }

        public override int AppVersionCode
        {
            get
            {
                float version = 0;
                if (float.TryParse(AppVersion, out version))
                    return (int)version * 10;
                return 0;
            }
            protected set
            {
                base.AppVersionCode = value;
            }
        }
    }
}

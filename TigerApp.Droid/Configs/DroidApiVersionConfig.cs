using System;
using AD.Plugins.CurrentActivity;
using Android.Content;
using Android.Content.PM;
using TigerApp.Shared.Services.API;

namespace TigerApp.Droid.Services
{
    public class DroidApiVersionConfig : BaseApiVersionConfig
    {
        public override string AppVersion
        {
            get
            {
                var currentContext = AD.Resolver.Resolve<ICurrentActivity>().Activity;
                if (currentContext == null)
                    return "1.0";
                return currentContext.PackageManager.GetPackageInfo(currentContext.PackageName,0).VersionName;
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
                var currentContext = AD.Resolver.Resolve<ICurrentActivity>().Activity;
                if (currentContext == null)
                    return 1;
                return currentContext.PackageManager.GetPackageInfo(currentContext.PackageName, 0).VersionCode;
            }
            protected set
            {
                base.AppVersionCode = value;
            }
        }
    }
}

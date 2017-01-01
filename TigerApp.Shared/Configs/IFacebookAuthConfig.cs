using System;

namespace TigerApp.Shared.Config
{
    public interface IFacebookAuthConfig
    {
        string AppId { get; }
        string AppName { get; }
        string[] ReadPermissions { get; }
    }

    public class FacebookAuthConfig : IFacebookAuthConfig
    {
        public string[] ReadPermissions => new string[]
        {
            "public_profile"
        };
        public string AppId => Constants.FacebookAppId;
        public string AppName => Constants.FacebookAppName;
    }
}
using System;
using AD;
using AD.Plugins.TripleDesAuthToken;

namespace TigerApp.Shared.Config
{
    public class TDesAuthServerConfig : TDesAuthTokenServerConfigBase
    {
        public TDesAuthServerConfig()
        {
            DeviceAuthRequestPath = "/api/v1/devices/";
            UserAuthRequestPath = "/api/v1/authuser/";
        }
    }
}

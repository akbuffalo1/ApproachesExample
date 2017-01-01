using System;
using AD.Plugins.Network;
using AD.Plugins.Network.Rest;

namespace TigerApp.Shared.Config
{
    public class HttpServerConfig : ServerConfigBase
    {
        
        public HttpServerConfig()
        {
            Protocol = HttpProtocol.Https;
            Server = "tigerapp-dev.devincloud.net";
            Port = 443;

        }
    }
}
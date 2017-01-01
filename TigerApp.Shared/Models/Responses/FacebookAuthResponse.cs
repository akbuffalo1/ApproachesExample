using System;

namespace TigerApp.Shared.Models.Responses
{
    public class FacebookAuthResponse
    {
        public string Token { get; internal set; }
        public string DisplayName { get; internal set; }
    }
}
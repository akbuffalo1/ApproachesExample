using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models.Responses
{
    public class BaseLoginResponse
    {
        [JsonProperty("auth_token")]
        public string Token { get; set; }
    }
}

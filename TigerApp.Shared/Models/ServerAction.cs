using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models
{
    public class ServerAction
    {
       [JsonProperty("action")]
        public string Action;
        [JsonProperty("path")]
        public string Path;
        [JsonProperty("value")]
        public string Value;
    }
}


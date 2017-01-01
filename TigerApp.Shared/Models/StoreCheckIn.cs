using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models
{
    public class StoreCheckIn
    {
        [JsonProperty("place")]
        public string StoreId { get; set; }
        [JsonProperty("count")]
        public string CheckInCount { get; set; }
    }
}

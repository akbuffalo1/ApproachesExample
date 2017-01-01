using Newtonsoft.Json;
using System;

namespace TigerApp.Shared.Models
{
    public class Coupon
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("special")]
        public bool Special { get; set; }

        [JsonProperty("mark")]
        public string Mark { get; set; }
    }
}
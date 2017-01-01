using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models
{
    public class Badge
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public bool Achieved { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("mark")]
        public string Mark { get; set; }
    }
}
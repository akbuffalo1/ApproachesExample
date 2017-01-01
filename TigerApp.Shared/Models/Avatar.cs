using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models
{
    public class Avatar
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("mark")]
        public string Mark { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }
    }
}

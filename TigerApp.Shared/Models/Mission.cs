using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TigerApp.Shared.Models
{
    public class Objective
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("prize_units")]
        public string PrizeUnits { get; set; }

        [JsonProperty("completed")]
        public bool Completed { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonProperty("mark")]
        public string Mark { get; set; }
    }

    public class Mission
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("goals")]
        public int Goals { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonProperty("objectives")]
        public List<Objective> Objectives { get; set; }

        [JsonProperty("mark")]
        public string Mark { get; set; }
    }
}
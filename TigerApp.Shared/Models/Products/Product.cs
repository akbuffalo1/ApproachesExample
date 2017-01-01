using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TigerApp.Shared.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("vote")]
        public string Vote { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("product_page_link")]
        public string SiteLink { get; set; }
    }
}

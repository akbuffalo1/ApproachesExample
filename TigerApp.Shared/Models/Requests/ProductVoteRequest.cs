using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TigerApp.Shared.Models.Requests
{
    public class ProductVoteRequest
    {
        [JsonIgnore]
        public long Id { get; set; }

        [JsonProperty("value")]
        public ProductVote Vote { get; set; }
    }
}

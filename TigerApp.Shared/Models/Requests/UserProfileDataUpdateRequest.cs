using System;
using Newtonsoft.Json;
namespace TigerApp.Shared.Models.Requests
{
    public class UserProfileDataUpdateRequest
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("nickname")]
        public string NickName { get; set; }

        [JsonProperty("mobile_number")]
        public string MobileNumber { get; set; }

        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        [JsonProperty("city")]
        public string TigerCity { get; set; }
    }
}

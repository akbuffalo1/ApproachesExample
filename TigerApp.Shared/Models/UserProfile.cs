using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TigerApp.Shared.Models
{
    public class UserProfile
    {
        [JsonIgnore]
        public bool IsEmpty = false;

        public static UserProfile Empty => new UserProfile() { IsEmpty = true };

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

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }

        [JsonProperty("badges")]
        public List<Badge> Badges { get; set; }

        [JsonProperty("avatars")]
        public List<Avatar> Avatars { get; set; }
    }
}

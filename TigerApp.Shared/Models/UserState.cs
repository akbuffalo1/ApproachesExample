using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TigerApp.Shared.Models
{

    public class Current
    {
        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("points")]
        public int Points { get; set; }

        [JsonProperty("missions_count")]
        public int MissionsCount { get; set; }

        [JsonProperty("coupon_count")]
        public int CouponCount { get; set; }
    }

    public class LevelThreshold
    {
        [JsonProperty("points")]
        public int Points { get; set; }

        [JsonProperty("missions_count")]
        public int MissionsCount { get; set; }
    }

    public class CouponThreshold
    {
        [JsonProperty("points")]
        public int Points { get; set; }
    }

    public class UserState
    {
        [JsonProperty("current")]
        public Current Current { get; set; }

        [JsonProperty("level_threshold")]
        public LevelThreshold LevelThreshold { get; set; }

        [JsonProperty("coupon_threshold")]
        public CouponThreshold CouponThreshold { get; set; }
    }
}

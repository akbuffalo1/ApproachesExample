using System;
using Newtonsoft.Json;
using TigerApp.Shared.Models.TrackedActions;

namespace TigerApp.Shared.Models.Proximity
{
    public class DistanceUnit
    {
        public const string Meters = "mt";
        public const string Centimeters = "cm";
        public const string Feet = "ft";
        public const string Inches = "in";
    }

    public class ProximityEvent
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }
        [JsonProperty("place")]
        public Place Place { get; set; }
        [JsonProperty("distance")]
        public int Distance { get; set; } 
        [JsonProperty("tollerance")]
        public int Tollerance { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; } 
    }
}

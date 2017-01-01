using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/* Generated with http://json2csharp.com */
namespace TigerApp.Shared.Models
{
    public class Region
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class City
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public Region Region { get; set; }
    }

    public class Location
    {
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("radius")]
        public string Radius { get; set; }

        [JsonProperty("abroad")]
        public string Abroad { get; set; }

        [JsonProperty("city")]
        public City City { get; set; }
    }

    public class OpeningHour
    {
        [JsonProperty("weekday")]
        public string Weekday { get; set; }

        [JsonProperty("from_hour")]
        public string FromHour { get; set; }

        [JsonProperty("to_hour")]
        public string ToHour { get; set; }
    }

    public class Store
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("mob_phone")]
        public string MobPhone { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonIgnore]
        public List<OpeningHour> OpeningHours { get; set; }

        [JsonProperty("opening_hours_text")]
        public string OpeningHoursText { get; set; }

        [JsonIgnore]
        public int DistanceInMeters { get; set; }
    }
}
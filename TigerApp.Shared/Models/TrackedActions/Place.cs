using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models.TrackedActions
{

    public class Place
    {
        [JsonProperty("place_id")]
        public string PlaceId;
        [JsonProperty("city")]
        public string City;
        [JsonProperty("region")]
        public string Region;
        [JsonProperty("state")]
        public string State;
        [JsonProperty("latitude")]
        public double Latitude;
        [JsonProperty("longitude")]
        public double Longitude;

        public Place(Store store)
        {
            PlaceId = store.Slug;
            City = store.Location.City.Name;
            Region = store.Location.City.Region.Name;
            State = "Italy";
            Latitude = store.Location.Latitude.Value;
            Longitude = store.Location.Longitude.Value;
        }

        public Place() { }
    }
}

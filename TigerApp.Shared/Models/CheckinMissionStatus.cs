using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace TigerApp.Shared.Models
{
    public class CheckinMissionStatus
    {
        [JsonProperty("checkin_stores")]
        public List<string> StoresCheckIn;
    }

    public class CheckinCityMissionStatus
    {
        [JsonProperty("checkin_cities")]
        public List<string> CheckInCities;
    }
}

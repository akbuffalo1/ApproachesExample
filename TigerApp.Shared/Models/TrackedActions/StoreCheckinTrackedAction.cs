using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models.TrackedActions
{

    public class StoreCheckInTrackedActionPayload : TrackedActionPayload {
       [JsonProperty("store_id")]
        public string StoreId; 
        [JsonProperty("place")]
        public Place Place;
    }

    public class StoreCheckInTrackedAction:TrackedAction
    {
        public StoreCheckInTrackedAction(Store store):base("store-checkin", new StoreCheckInTrackedActionPayload() { StoreId = store.Slug, Place = new Place(store) })
        {
        }
    }
}

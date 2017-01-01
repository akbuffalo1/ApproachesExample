using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models.TrackedActions
{
    public class ShareFBTrackedAction:TrackedAction
    {
        public class ShareFBTrackedActionPayload : TrackedActionPayload
        {
            [JsonProperty("product")]
            public string ProductId;
        }

        public ShareFBTrackedAction(string productId):base("product-share-fb", new ShareFBTrackedActionPayload() { ProductId = productId})
        {
            
        }
    }
}

using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models.TrackedActions
{
    public abstract class TrackedActionPayload { 
    }

    public class TrackedAction
    {
        [JsonProperty("action")]
        public string Action;
        [JsonProperty("payload")]
        public TrackedActionPayload Payload;

        public TrackedAction(string action,TrackedActionPayload payload = null)
        {
            Action = action;
            Payload = payload;
        }
    }
}

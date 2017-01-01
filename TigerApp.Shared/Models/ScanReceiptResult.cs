using System;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models
{
    public class ScanReceiptResult
    {
        public static ScanReceiptResult Empty = new ScanReceiptResult() { 
            ReceiptId = -1,
            Amount = 0
        };

        [JsonProperty("receipt_id")]
        public long ReceiptId { get; set; }
        [JsonProperty("amount")]
        public float Amount { get; set; }
    }
}


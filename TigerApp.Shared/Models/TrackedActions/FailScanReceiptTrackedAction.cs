using System;
using AD.Plugins.DeviceInfo;
using Newtonsoft.Json;

namespace TigerApp.Shared.Models.TrackedActions
{
    public class FailScanReceiptTrackedActionPayload : TrackedActionPayload
    {
        [JsonProperty("receipt_id")]
        public long ReceiptId { get; set; }
        [JsonProperty("amount")]
        public float Amount { get; set; }
        [JsonProperty("scan_image")]
        public string EncodedImage { get; set; }
        [JsonProperty("device")]
        public string Device { get; set; }
        [JsonProperty("os")]
        public string OS { get; set; }
        [JsonProperty("os_version")]
        public string OSVersion { get; set; }

        public FailScanReceiptTrackedActionPayload() { 
            var devInfo = AD.Resolver.Resolve<IDeviceInfo>();
            Device = $"{devInfo.Manufacturer}|{devInfo.Model}";
            OS = devInfo.Manufacturer.Equals("Apple") ? "iOS" : "Android";
            OSVersion = devInfo.SystemVersion;
        }
    }


    public class FailScanReceiptTrackedAction:TrackedAction
    {
        public FailScanReceiptTrackedAction(long receiptId, float amount, string encodedImage):base("scan-failed", new FailScanReceiptTrackedActionPayload() { 
            ReceiptId = receiptId,
            Amount = amount,
            EncodedImage = encodedImage
        })
        {
        }
    }
}

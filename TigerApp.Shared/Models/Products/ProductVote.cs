using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TigerApp.Shared.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductVote
    {
        [EnumMember(Value = "like")]
        Like,
        [EnumMember(Value = "dislike")]
        Dislike,
        [EnumMember(Value = "ignore")]
        Ignore,
        [EnumMember(Value = "share")]
        Share
    }
}

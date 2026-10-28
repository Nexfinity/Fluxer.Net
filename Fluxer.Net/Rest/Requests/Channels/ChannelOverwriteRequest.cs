using Fluxer.Net.Extensions;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class ChannelOverwriteRequest
{
    [JsonProperty("id")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public OverwriteRequestType Type { get; set; }

    [JsonProperty("allow")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Allow { get; set; }

    [JsonProperty("deny")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Deny { get; set; }
}

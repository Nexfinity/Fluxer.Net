using Newtonsoft.Json;

namespace Fluxer.Net;

public class ChannelOverwriteRequest
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public ChannelOverwriteRequestType Type { get; set; }

    [JsonProperty("allow")]
    public ulong Allow { get; set; }

    [JsonProperty("deny")]
    public ulong Deny { get; set; }
}

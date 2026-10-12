using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class CallDeleteGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("unavailable")]
    public bool? IsUnavailable { get; set; }
}
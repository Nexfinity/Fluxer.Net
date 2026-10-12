using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ChannelPinsAckGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("timestamp")]
    public DateTimeOffset Timestamp { get; set; }
}
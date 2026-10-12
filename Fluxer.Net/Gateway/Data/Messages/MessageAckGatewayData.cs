using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class MessageAckGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }
}
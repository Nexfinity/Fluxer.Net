using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class MessageAckGatewayData : IGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }
}

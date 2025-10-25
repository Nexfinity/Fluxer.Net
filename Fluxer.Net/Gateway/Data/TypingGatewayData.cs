using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class TypingGatewayData : IGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("community_id")]
    public ulong CommunityId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("timestamp")]
    public ulong Timestamp { get; set; }
}

using Newtonsoft.Json;

namespace Squll.Net.Gateway.Data;

public class TypingGatewayData : IGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("timestamp")]
    public ulong Timestamp { get; set; }
}

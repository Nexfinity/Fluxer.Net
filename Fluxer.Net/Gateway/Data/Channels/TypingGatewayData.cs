using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class TypingGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    [JsonIgnore]
    public DateTimeOffset TimestampAt => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp);

    [JsonProperty("member")]
    public GuildMemberGatewayData? Member { get; set; }
}
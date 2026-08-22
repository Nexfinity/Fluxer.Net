using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class TypingGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// Unix timestamp in milliseconds when the user started typing.
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// The <see cref="Timestamp"/> value converted to a UTC <see cref="DateTime"/>.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset TimestampUtc => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;

    /// <summary>
    /// Guild member data for the typing user (only present in guild channels).
    /// </summary>
    [JsonProperty("member")]
    public GuildMemberGatewayData? Member { get; set; }
}

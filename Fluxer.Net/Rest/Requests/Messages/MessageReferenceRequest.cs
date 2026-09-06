using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class MessageReferenceRequest
{
    /// <summary>
    /// ID of the message being referenced
    /// </summary>
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    /// <summary>
    /// ID of the channel containing the referenced message
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// ID of the guild containing the referenced message
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <summary>
    /// Type of reference
    /// </summary>
    [JsonProperty("type")]
    public MessageReferenceType Type { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L62"/>
/// </remarks>
public class MessageReferenceResponse
{
    /// <summary>
    /// The ID of the channel containing the referenced message
    /// </summary>
    [JsonRequired]
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// The ID of the referenced message
    /// </summary>
    [JsonRequired]
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    /// <summary>
    /// The ID of the guild containing the referenced message
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
    
    [JsonRequired]
    [JsonProperty("type")]
    public MessageReferenceType Type { get; set; }
}

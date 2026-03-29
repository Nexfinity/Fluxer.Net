using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/SharedMessageSchemas.tsx#L43"/>
/// </remarks>
public class MessageReferenceRequest
{
    /// <summary>
    /// ID of the message being referenced
    /// </summary>
    [JsonRequired]
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    /// <summary>
    /// ID of the channel containing the referenced message
    /// </summary>
    [JsonRequired]
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
    [JsonRequired]
    [JsonProperty("type")]
    public MessageReferenceType Type { get; set; }
}

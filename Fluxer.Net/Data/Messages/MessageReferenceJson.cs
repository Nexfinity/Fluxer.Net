using Newtonsoft.Json;

namespace Fluxer.Net;


/// <inheritdoc />
public class MessageReferenceJson : IMessageReference
{
    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public MessageReferenceType Type { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class SavedMessageJson : ISavedMessage
{
    /// <inheritdoc />
    [JsonProperty("message_id")]
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("saved_at")]
    public DateTimeOffset SavedAt { get; set; }
}

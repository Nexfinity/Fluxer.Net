using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A webhook referenced by audit log entries, as the API sends it.
/// </summary>
public class AuditLogWebhookJson : ISnowflake
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The type of the webhook.
    /// </summary>
    [JsonProperty("type")]
    public WebhookType Type { get; set; }

    /// <summary>
    /// The id of the guild the webhook belongs to.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <summary>
    /// The id of the channel the webhook posts to.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// The name of the webhook.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The avatar hash of the webhook.
    /// </summary>
    [JsonProperty("avatar_hash")]
    public string? AvatarHash { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
}

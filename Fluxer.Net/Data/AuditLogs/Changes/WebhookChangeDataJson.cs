using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class WebhookChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the webhook.
    /// </summary>
    [JsonProperty("id")]
    public ulong WebhookId { get; set; }

    /// <summary>
    /// The id of the guild the webhook belongs to.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The id of the channel the webhook posts to.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// The name of the webhook.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The id of the user that created the webhook.
    /// </summary>
    [JsonProperty("creator_id")]
    public ulong? CreatorId { get; set; }

    /// <summary>
    /// The avatar hash of the webhook.
    /// </summary>
    [JsonProperty("avatar_hash")]
    public string? AvatarHash { get; set; }
}

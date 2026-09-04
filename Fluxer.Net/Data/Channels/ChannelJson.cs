using Newtonsoft.Json;

namespace Fluxer.Net;


/// <inheritdoc />
public class ChannelJson : IChannel
{

    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    [JsonIgnore]
    public string Mention => $"<#{Id}>";

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public ChannelType Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("topic")]
    public string? Topic { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon")]
    public string? IconHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    /// <inheritdoc />
    [JsonProperty("position")]
    public int Position { get; set; }

    /// <inheritdoc />
    [JsonProperty("owner_id")]
    public ulong? OwnerId { get; set; }

    /// <inheritdoc />
    [JsonProperty("recipient_ids")]
    public HashSet<ulong>? RecipientIds { get; set; }

    /// <inheritdoc />
    [JsonProperty("nsfw")]
    public bool IsNsfw { get; set; }

    /// <inheritdoc />
    [JsonProperty("rate_limit_per_user")]
    public int RateLimitPerUser { get; set; }

    /// <inheritdoc />
    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    /// <inheritdoc />
    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    /// <inheritdoc />
    [JsonProperty("rtc_region")]
    public string? RtcRegion { get; set; }

    /// <inheritdoc />
    [JsonProperty("last_message_id")]
    public ulong? LastMessageId { get; set; }

    /// <inheritdoc />
    [JsonProperty("last_pin_timestamp")]
    public DateTimeOffset? LastPinAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("permission_overwrites")]
    public List<PermissionOverwriteJson>? PermissionOverwrites { get; set; }

    /// <inheritdoc />
    [JsonProperty("nicks")]
    public Dictionary<string, string>? Nicknames { get; set; }

    /// <inheritdoc />
    [JsonProperty("soft_deleted")]
    public bool IsSoftDeleted { get; set; }

    /// <inheritdoc />
    [JsonProperty("indexed_at")]
    public DateTimeOffset? IndexedAt { get; set; }

    IEnumerable<IPermissionOverwrite>? IChannel.PermissionOverwrites => PermissionOverwrites;

    /// <inheritdoc/>
    public bool IsTextable => Channel.TextableTypes(Type);
}

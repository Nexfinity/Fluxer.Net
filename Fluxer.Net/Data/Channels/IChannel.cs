namespace Fluxer.Net;

public interface IChannel
{
    /// <summary>
    /// The unique identifier (snowflake) for this channel.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// Get the mention for this channel.
    /// </summary>
    string Mention { get; }

    /// <summary>
    /// The unique identifier (snowflake) for guild of this channel.
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    /// The type of the channel.
    /// </summary>
    ChannelType Type { get; }

    /// <summary>
    /// The name of the channel.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// The topic of the channel.
    /// </summary>
    string? Topic { get; }

    /// <summary>
    /// The icon hash of the channel (for group DMs).
    /// </summary>
    string? IconHash { get; }

    /// <summary>
    /// The URL associated with the channel.
    /// </summary>
    string? Url { get; }

    /// <summary>
    /// The ID of the parent category for this channel.
    /// </summary>
    ulong? ParentId { get; }

    /// <summary>
    /// The position of the channel relative to other channels.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// The ID of the owner of the channel. (for group DMs)
    /// </summary>
    ulong? OwnerId { get; }

    /// <summary>
    /// The recipients of the DM channel.
    /// </summary>
    HashSet<ulong>? RecipientIds { get; }

    /// <summary>
    /// Whether the channel is marked as NSFW.
    /// </summary>
    bool IsNsfw { get; }

    /// <summary>
    /// The slowmode for this channel to limit messages per second.
    /// </summary>
    int RateLimitPerUser { get; }

    /// <summary>
    /// The bitrate of the voice channel in bits per second.
    /// </summary>
    int? Bitrate { get; }

    /// <summary>
    /// The maximum number of users allowed in the voice channel.
    /// </summary>
    int? UserLimit { get; }

    /// <summary>
    /// The voice region ID for the voice channel.
    /// </summary>
    string? RtcRegion { get; }

    /// <summary>
    /// The ID of the last message sent in this channel.
    /// </summary>
    ulong? LastMessageId { get; }

    /// <summary>
    /// The ISO 8601 timestamp of when the last pinned message was pinned.
    /// </summary>
    DateTime? LastPinTimestamp { get; }

    /// <summary>
    /// The permission overwrites for this channel.
    /// </summary>
    IEnumerable<IPermissionOverwrite>? PermissionOverwrites { get; }

    /// <summary>
    /// Custom nicknames for users in this channel (for group DMs)
    /// </summary>
    Dictionary<string, string>? Nicknames { get; }

    bool IsSoftDeleted { get; }

    DateTime? IndexedAt { get; }
}

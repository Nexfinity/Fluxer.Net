namespace Fluxer.Net;

/// <summary>
/// A channel in a guild, a group or a direct message.
/// </summary>
public interface IChannel : IPartialChannel
{
    /// <summary>
    /// The ID of the guild the channel belongs to.
    /// </summary>
    ulong? GuildId { get; }

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
    int? Position { get; }

    /// <summary>
    /// The ID of the owner of the channel, for group DMs.
    /// </summary>
    ulong? OwnerId { get; }

    /// <summary>
    /// The IDs of the recipients of a DM or group DM.
    /// </summary>
    HashSet<ulong>? RecipientIds { get; }

    /// <summary>
    /// Whether the channel is marked as NSFW.
    /// </summary>
    bool IsNsfw { get; }

    /// <summary>
    /// The slowmode delay in seconds between messages from one user.
    /// </summary>
    int? RateLimitPerUser { get; }

    /// <summary>
    /// Metadata for a thread.
    /// </summary>
    IThreadMetadata? ThreadMetadata { get; }

    /// <summary>
    /// The bitrate of the voice channel in bits per second.
    /// </summary>
    int? Bitrate { get; }

    /// <summary>
    /// The maximum number of connections a user can have in the voice channel.
    /// </summary>
    int? VoiceConnectionLimit { get; }

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
    DateTimeOffset? LastPinAt { get; }

    /// <summary>
    /// The permission overwrites for this channel.
    /// </summary>
    IEnumerable<IPermissionOverwrite>? PermissionOverwrites { get; }

    /// <summary>
    /// Custom nicknames for users in this channel, for group DMs.
    /// </summary>
    Dictionary<string, string>? Nicknames { get; }

    /// <summary>
    /// The content warning level of the channel.
    /// </summary>
    GuildContentWarning? ContentWarningLevel { get; }

    /// <summary>
    /// The text shown with the content warning.
    /// </summary>
    string? ContentWarningText { get; }

    /// <summary>
    /// Whether the channel is soft deleted.
    /// </summary>
    bool IsSoftDeleted { get; }

    /// <summary>
    /// The time the channel was indexed.
    /// </summary>
    DateTimeOffset? IndexedAt { get; }

    /// <summary>
    /// The current user's membership in the thread.
    /// </summary>
    IThreadMember? ThreadMember { get; }

    /// <summary>
    /// The flags set on the channel.
    /// </summary>
    ChannelFlags Flags { get; }

    /// <summary>
    /// The tags a forum or media channel offers for its posts.
    /// </summary>
    IForumTag[]? ForumTags { get; }

    /// <summary>
    /// The IDs of the tags applied to a forum or media post.
    /// </summary>
    ulong[]? AppliedTags { get; }

    /// <summary>
    /// The emoji a forum shows as the default reaction on its posts.
    /// </summary>
    ForumReactionEmoji? DefaultReactionEmoji { get; }

    /// <summary>
    /// The default sort order of a forum's posts.
    /// </summary>
    ForumSortOrder? DefaultSortOrder { get; }

    /// <summary>
    /// The default layout of a forum's posts.
    /// </summary>
    ForumLayout? DefaultLayout { get; }

    /// <summary>
    /// The default time before a new thread is archived.
    /// </summary>
    ThreadArchiveDuration? DefaultAutoArchiveDuration { get; }

    /// <summary>
    /// The default slowmode delay in seconds for new threads.
    /// </summary>
    int? DefaultRateLimitPerUser { get; }
}

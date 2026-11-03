using Fluxer.Net.Gateway;

namespace Fluxer.Net;

/// <summary>
/// Settings for the gateway connection. Some of them are sent to Fluxer when the session identifies.
/// </summary>
public class FluxerGatewayConfig
{
    /// <summary>
    /// The gateway event types Fluxer should not send, such as PRESENCE_UPDATE or TYPING_START. Empty by default.
    /// </summary>
    public List<string> IgnoredEvents { get; set; } = new();

    /// <summary>
    /// The presence sent when connecting. Null sends none.
    /// </summary>
    public PresenceUpdateGatewayData? Presence { get; set; } = null;

    /// <summary>
    /// Whether guild emojis are not cached. GUILD_EMOJIS_UPDATE is then ignored, so the emoji created, updated and
    /// deleted events are not raised.
    /// </summary>
    public bool DisableEmojiCache { get; set; }

    /// <summary>
    /// Whether guild stickers are not cached. GUILD_STICKERS_UPDATE is then ignored, so the sticker created, updated
    /// and deleted events are not raised.
    /// </summary>
    public bool DisableStickerCache { get; set; }

    /// <summary>
    /// Whether the gateway batches reactions added to the same message into MESSAGE_REACTION_ADD_MANY, raised as
    /// <see cref="FluxerGatewayClient.MessageReactionsAdded"/>. A batch of one is still sent as MESSAGE_REACTION_ADD.
    /// </summary>
    public bool DebounceMessageReactions { get; set; }

    /// <summary>
    /// Whether the members of every guild are downloaded as it becomes available, so the member cache holds them all.
    /// Guilds are downloaded a few at a time and retried when Fluxer does not send them in time. Off by default.
    /// </summary>
    public bool DownloadAllMembers { get; set; }

    /// <summary>
    /// How many guilds have their members downloaded at once when <see cref="DownloadAllMembers"/> is on. Defaults to 2.
    /// </summary>
    public int MemberDownloadConcurrency { get; set; } = 2;

    /// <summary>
    /// How many times a guild's member download is tried before it is given up and logged. Defaults to 4.
    /// </summary>
    public int MemberDownloadAttempts { get; set; } = 4;

    /// <summary>
    /// The cache of the state from before update and delete events. Off by default.
    /// </summary>
    public FluxerCacheConfig Cache { get; set; } = new FluxerCacheConfig();

    /// <summary>
    /// How gateway events reach your handlers. Ordered per guild by default.
    /// </summary>
    public FluxerEventConfig Events { get; set; } = new FluxerEventConfig();
}

namespace Fluxer.Net;

/// <summary>
/// A guild with its settings and member counts.
/// </summary>
public interface IGuild : IPartialGuild
{
    /// <summary>
    /// The ID of the guild owner.
    /// </summary>
    ulong OwnerId { get; }

    /// <summary>
    /// The vanity URL code for the guild.
    /// </summary>
    string? VanityUrlCode { get; }

    /// <summary>
    /// The verification level required for members to participate.
    /// </summary>
    GuildVerificationLevel VerificationLevel { get; }

    /// <summary>
    /// The MFA level required for moderation actions.
    /// </summary>
    GuildMfaLevel MfaLevel { get; }

    /// <summary>
    /// The NSFW level of the guild.
    /// </summary>
    GuildNsfwLevel NsfwLevel { get; }

    /// <summary>
    /// The explicit content filter level of the guild.
    /// </summary>
    GuildContentFilter ExplicitContentFilter { get; }

    /// <summary>
    /// The default notification level for new members.
    /// </summary>
    GuildDefaultNotifications DefaultMessageNotifications { get; }

    /// <summary>
    /// The ID of the channel where system messages are sent.
    /// </summary>
    ulong? SystemChannelId { get; }

    /// <summary>
    /// System channel message flags.
    /// </summary>
    SystemChannelFlags SystemChannelFlags { get; }

    /// <summary>
    /// The ID of the rules channel.
    /// </summary>
    ulong? RulesChannelId { get; }

    /// <summary>
    /// The ID of the AFK voice channel.
    /// </summary>
    ulong? AfkChannelId { get; }

    /// <summary>
    /// AFK timeout in seconds before moving users to the AFK channel.
    /// </summary>
    int AfkTimeout { get; }

    /// <summary>
    /// Bitmask of disabled guild operations.
    /// </summary>
    GuildOperations DisabledOperations { get; }

    /// <summary>
    /// How far back members without Read Message History can read messages, or null when they cannot read any message history.
    /// </summary>
    DateTimeOffset? MessageHistoryCutoff { get; }

    /// <summary>
    /// Whether the guild is marked as NSFW.
    /// </summary>
    bool IsNsfw { get; }

    /// <summary>
    /// The content warning setting of the guild.
    /// </summary>
    GuildContentWarning ContentWarningLevel { get; }

    /// <summary>
    /// The custom warning text shown to new members.
    /// </summary>
    string ContentWarningText { get; }

    /// <summary>
    /// The number of online members in the guild.
    /// </summary>
    int? OnlineCount { get; }

    /// <summary>
    /// The total number of members in the guild.
    /// </summary>
    int? MemberCount { get; }

    /// <summary>
    /// Approximate total member count.
    /// </summary>
    /// <remarks>
    /// Only filled when the guilds were requested with counts.
    /// </remarks>
    int? ApproximateMemberCount { get; }

    /// <summary>
    /// Approximate online member count.
    /// </summary>
    /// <remarks>
    /// Only filled when the guilds were requested with counts.
    /// </remarks>
    int? ApproximatePresenceCount { get; }

    /// <summary>
    /// Whether threads, forum and media channels are active in this guild for the requesting client. Only sent on the current user guild list.
    /// </summary>
    bool? ThreadsActive { get; }

    /// <summary>
    /// Whether the requesting user is visiting the guild without being a member.
    /// </summary>
    bool? Visiting { get; }
}

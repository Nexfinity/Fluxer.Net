namespace Fluxer.Net;

public interface IGuild
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
    /// Required verification level for members to participate.
    /// </summary>
    int VerificationLevel { get; }

    /// <summary>
    /// Required MFA level for moderation actions.
    /// </summary>
    int MfaLevel { get; }

    /// <summary>
    /// The NSFW level of the guild.
    /// </summary>
    int NsfwLevel { get; }

    /// <summary>
    /// Level of content filtering for explicit media.
    /// </summary>
    int ExplicitContentFilter { get; }

    /// <summary>
    /// Default notification level for new members.
    /// </summary>
    int DefaultMessageNotifications { get; }

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
    int DisabledOperations { get; }

    int? MaxPresences { get; }

    int MemberCount { get; }

    DateTime? AuditLogsIndexedAt { get; }

    /// <summary>
    /// ISO8601 timestamp controlling how far back members without Read Message History can access messages. When null, no historical access is allowed.
    /// </summary>
    DateTime? MessageHistoryCutoff { get; }
}

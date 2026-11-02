namespace Fluxer.Net;

/// <summary>
/// An invite with its guild, channel and inviter.
/// </summary>
public interface IPartialInvite
{
    /// <summary>
    /// The unique invite code.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// The kind of destination the invite leads to.
    /// </summary>
    InviteType Type { get; }

    /// <summary>
    /// The guild this invite is for.
    /// </summary>
    IPartialGuild? Guild { get; }

    /// <summary>
    /// The channel this invite is for.
    /// </summary>
    IPartialChannel? Channel { get; }

    /// <summary>
    /// The user who created the invite.
    /// </summary>
    IUser? Inviter { get; }

    /// <summary>
    /// The approximate total member count of the guild.
    /// </summary>
    int MemberCount { get; }

    /// <summary>
    /// The approximate online member count of the guild.
    /// </summary>
    int PresenceCount { get; }

    /// <summary>
    /// The time the invite expires.
    /// </summary>
    DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    /// Whether the invite grants temporary membership.
    /// </summary>
    bool IsTemporary { get; }
}

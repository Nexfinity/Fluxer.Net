namespace Fluxer.Net;

public interface IPartialInvite
{
    /// <summary>
    /// The unique invite code.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// The type of invite (guild).
    /// </summary>
    InviteType Type { get; }

    /// <summary>
    /// The guild this invite is for
    /// </summary>
    IPartialGuild? Guild { get; }

    /// <summary>
    /// The channel this invite is for.
    /// </summary>
    IPartialChannel? Channel { get; }

    /// <summary>
    /// The user who created the invite.
    /// </summary>
    IUser Inviter { get; }

    /// <summary>
    /// The approximate total member count of the guild
    /// </summary>
    int MemberCount { get; }

    /// <summary>
    /// The approximate online member count of the guild.
    /// </summary>
    int PresenceCount { get; }

    /// <summary>
    /// ISO8601 timestamp of when the invite expires.
    /// </summary>
    DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    /// Whether the invite grants temporary membership
    /// </summary>
    bool IsTemporary { get; }
}

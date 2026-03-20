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
    int Type { get; }

    /// <summary>
    /// The guild this invite is for
    /// </summary>
    PartialGuildJson? Guild { get; }

    /// <summary>
    /// The channel this invite is for.
    /// </summary>
    InviteChannelJson? Channel { get; }

    /// <summary>
    /// The user who created the invite.
    /// </summary>
    InviteUserJson Inviter { get; }

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
    DateTime? ExpiresAt { get; }

    /// <summary>
    /// Whether the invite grants temporary membership
    /// </summary>
    bool Temporary { get; }
}

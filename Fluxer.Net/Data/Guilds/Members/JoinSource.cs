namespace Fluxer.Net;

public enum JoinSource
{
    /// <summary>
    /// User created the guild.
    /// </summary>
    Creator = 0,

    /// <summary>
    /// User joined by invite.
    /// </summary>
    Invite = 1,

    /// <summary>
    /// User joined by vanity url.
    /// </summary>
    VanityUrl = 2,

    /// <summary>
    /// Bot was manually added.
    /// </summary>
    BotInvite = 3,

    /// <summary>
    /// User was force joined by admin.
    /// </summary>
    AdminForceAdd = 4,
}

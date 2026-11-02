namespace Fluxer.Net;

/// <summary>
/// The kind of destination an invite leads to.
/// </summary>
public enum InviteType
{
    /// <summary>
    /// An invite to join a guild.
    /// </summary>
    Guild = 0,

    /// <summary>
    /// An invite to join a group DM.
    /// </summary>
    Group = 1,
}

namespace Fluxer.Net;

/// <summary>
/// The kind of target in a channel overwrite request.
/// </summary>
public enum OverwriteRequestType
{
    /// <summary>
    /// The overwrite applies to a role.
    /// </summary>
    Role = 0,

    /// <summary>
    /// The overwrite applies to a member.
    /// </summary>
    Member = 1
}

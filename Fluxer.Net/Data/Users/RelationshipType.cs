namespace Fluxer.Net;

/// <summary>
/// The kind of relationship between the current user and another user.
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// The user is a friend.
    /// </summary>
    Friend = 1,

    /// <summary>
    /// The user is blocked.
    /// </summary>
    Blocked = 2,

    /// <summary>
    /// Incoming friend request from the user.
    /// </summary>
    IncomingRequest = 3,

    /// <summary>
    /// The current user has sent a friend request to the user.
    /// </summary>
    OutgoingRequest = 4,
}

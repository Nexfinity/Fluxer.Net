namespace Fluxer.Net;

/// <summary>
/// Flags that control who can send friend requests to a user. Multiple sources can be combined using bitwise OR.
/// </summary>
[Flags]
public enum FriendSourceFlags
{
    /// <summary>
    /// No sources enabled (no one can send friend requests).
    /// </summary>
    None = 0,

    /// <summary>
    /// Allow friend requests from users who share mutual friends.
    /// </summary>
    MutualFriends = 1 << 0,

    /// <summary>
    /// Allow friend requests from users who share mutual guilds/servers.
    /// </summary>
    MutualGuilds = 1 << 1,

    /// <summary>
    /// Allow friend requests from users with no relation (anyone).
    /// </summary>
    NoRelation = 1 << 2,
}

namespace Fluxer.Net;

/// <summary>
/// A ban of a user from a guild.
/// </summary>
public interface IGuildBan
{
    /// <summary>
    /// When the member was banned.
    /// </summary>
    DateTimeOffset BannedAt { get; }

    /// <summary>
    /// When the ban expires, or null when it does not expire.
    /// </summary>
    DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    /// The ID of the user who issued the ban.
    /// </summary>
    ulong ModeratorId { get; }

    /// <summary>
    /// The reason for the ban, up to 512 characters.
    /// </summary>
    string? Reason { get; }

    /// <summary>
    /// The user that was banned.
    /// </summary>
    IUser User { get; }
}

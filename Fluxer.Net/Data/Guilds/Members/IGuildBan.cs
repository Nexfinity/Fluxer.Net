namespace Fluxer.Net;

public interface IGuildBan
{
    /// <summary>
    /// When the member was banned.
    /// </summary>
    DateTime BannedAt { get; }

    /// <summary>
    /// When the ban expires (<see langword="null"/> for never)
    /// </summary>
    DateTime? ExpiresAt { get; }

    /// <summary>
    /// Id of the user who issues the ban.
    /// </summary>
    ulong ModeratorId { get; }

    /// <summary>
    /// Ban Reason (max 512 characters)
    /// </summary>
    string? Reason { get; }

    /// <summary>
    /// The user that was banned.
    /// </summary>
    IUser User { get; }
}

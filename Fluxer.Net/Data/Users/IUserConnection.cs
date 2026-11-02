namespace Fluxer.Net;

/// <summary>
/// An account or domain that a user has connected to their profile.
/// </summary>
public interface IUserConnection
{
    /// <summary>
    /// The unique identifier for this connection.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The type of connection.
    /// </summary>
    /// <remarks>
    /// <c>bluesky</c> or <c>domain</c>.
    /// </remarks>
    string Type { get; }

    /// <summary>
    /// The handle or domain of the connection.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether the connection has been verified.
    /// </summary>
    bool IsVerified { get; }

    /// <summary>
    /// Bitfield controlling who can see this connection.
    /// </summary>
    ulong VisibilityFlags { get; }

    /// <summary>
    /// The display order of this connection.
    /// </summary>
    int SortOrder { get; }
}

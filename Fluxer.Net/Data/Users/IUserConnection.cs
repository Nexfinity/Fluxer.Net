namespace Fluxer.Net;

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
    /// bsky or domain
    /// </remarks>
    string Type { get; }

    /// <summary>
    /// The display name of the connection. (handle or domain)
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

namespace Fluxer.Net;

/// <summary>
/// A signed-in session on a device, with its client and IP details.
/// </summary>
public interface IAuthSession
{
    /// <summary>
    /// Hashed session identifier. (base64url)
    /// </summary>
    byte[] SessionIdHash { get; }

    /// <summary>
    /// ISO timestamp when the session was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// The approximate time the session was last used.
    /// </summary>
    DateTimeOffset ApproximateLastUsedAt { get; }

    /// <summary>
    /// Client IP address.
    /// </summary>
    string ClientIp { get; }

    /// <summary>
    /// Reverse DNS hostname for the client IP (PTR), if available.
    /// </summary>
    string? ClientIpReverse { get; }

    /// <summary>
    /// Client operating system, if detected.
    /// </summary>
    string? ClientOs { get; }

    /// <summary>
    /// Client platform, if detected.
    /// </summary>
    string? ClientPlatform { get; }

    /// <summary>
    /// The approximate location of the client IP, if available.
    /// </summary>
    string? ClientCountry { get; }
}

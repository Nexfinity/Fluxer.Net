namespace Fluxer.Net;

public interface IAuthSession
{
    /// <summary>
    /// Hashed session identifier. (base64url)
    /// </summary>
    byte[] SessionIdHash { get; }

    /// <summary>
    /// ISO timestamp when the session was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// ISO timestamp of the session last usage. (approximate)
    /// </summary>
    DateTime ApproximateLastUsedAt { get; }

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
    /// Approximate geo location label for the client IP, if available.
    /// </summary>
    string? ClientCountry { get; }
}

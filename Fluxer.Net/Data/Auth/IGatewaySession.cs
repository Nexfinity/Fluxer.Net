namespace Fluxer.Net;

/// <summary>
/// A gateway session of the current user, with its presence status.
/// </summary>
public interface IGatewaySession
{
    /// <summary>
    /// The id of the gateway session.
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// The presence status of the session.
    /// </summary>
    string Status { get; }

    /// <summary>
    /// Whether the session is from a mobile device.
    /// </summary>
    bool IsMobile { get; }

    /// <summary>
    /// Whether the session is marked away from keyboard.
    /// </summary>
    bool IsAfk { get; }
}

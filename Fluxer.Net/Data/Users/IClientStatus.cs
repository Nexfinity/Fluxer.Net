namespace Fluxer.Net;

/// <summary>
/// The status of a user on each client platform.
/// </summary>
public interface IClientStatus
{
    /// <summary>
    /// The status on the desktop client, or null when the user is not connected through it.
    /// </summary>
    string? Desktop { get; }

    /// <summary>
    /// The status on the mobile client, or null when the user is not connected through it.
    /// </summary>
    string? Mobile { get; }

    /// <summary>
    /// The status on the web client, or null when the user is not connected through it.
    /// </summary>
    string? Web { get; }
}

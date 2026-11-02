namespace Fluxer.Net;

/// <summary>
/// An invite with its usage and expiry details.
/// </summary>
public interface IInvite
{
    /// <summary>
    /// The time the invite was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// The number of times this invite has been used.
    /// </summary>
    int Uses { get; }

    /// <summary>
    /// The maximum number of times this invite can be used.
    /// </summary>
    int MaxUses { get; }

    /// <summary>
    /// The duration in seconds before the invite expires.
    /// </summary>
    int MaxAge { get; }
}

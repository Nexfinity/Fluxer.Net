namespace Fluxer.Net;

public interface IInvite
{
    /// <summary>
    /// ISO8601 timestamp of when the invite was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// The number of times this invite has been used
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

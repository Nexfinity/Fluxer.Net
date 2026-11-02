namespace Fluxer.Net;

/// <summary>
/// The current user's eligibility to ring a call.
/// </summary>
public interface ICallEligibility
{
    /// <summary>
    /// Whether the current user can ring this call.
    /// </summary>
    bool IsRingable { get; }

    /// <summary>
    /// Whether the call is silent.
    /// </summary>
    bool IsSilent { get; }
}

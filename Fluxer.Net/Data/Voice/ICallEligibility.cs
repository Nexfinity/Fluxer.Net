namespace Fluxer.Net;

public interface ICallEligibility
{
    /// <summary>
    /// Whether the current user can ring this call
    /// </summary>
    bool IsRingable { get; }

    /// <summary>
    /// Whether the call should be joined silently
    /// </summary>
    bool IsSilent { get; }
}

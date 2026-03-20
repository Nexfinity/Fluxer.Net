namespace Fluxer.Net;

/// <summary>
/// Returns available voice and video calling regions for the channel, used to optimise connection quality.
/// </summary>
/// <remarks>
/// Requires Call permission.
/// </remarks>
public interface IRtcRegion
{
    /// <summary>
    /// The unique identifier for this RTC region.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The display name of the RTC region.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The emoji associated with this RTC region
    /// </summary>
    string Emoji { get; }
}
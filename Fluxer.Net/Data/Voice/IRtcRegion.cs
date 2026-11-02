namespace Fluxer.Net;

/// <summary>
/// A voice region a channel can use for calls.
/// </summary>
public interface IRtcRegion
{
    /// <summary>
    /// The unique identifier of the region.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The display name of the region.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The emoji associated with the region.
    /// </summary>
    string Emoji { get; }
}
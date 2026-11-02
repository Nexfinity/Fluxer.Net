namespace Fluxer.Net;

/// <summary>
/// The configuration of a mute.
/// </summary>
public interface IMuteConfiguration
{
    /// <summary>
    /// When the mute ends, or null when no end is set.
    /// </summary>
    DateTimeOffset? EndAt { get; }

    /// <summary>
    /// The selected mute duration, in seconds.
    /// </summary>
    int? SelectedTimeSeconds { get; }
}

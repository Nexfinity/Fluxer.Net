namespace Fluxer.Net;

/// <summary>
/// The current user's settings for one channel of a guild.
/// </summary>
public interface IGuildChannelOverride
{
    /// <summary>
    /// Whether the channel is collapsed.
    /// </summary>
    bool Collapsed { get; }

    /// <summary>
    /// The message notification level set for the channel, or null when none is set.
    /// </summary>
    int? MessageNotifications { get; }

    /// <summary>
    /// Whether the channel is muted.
    /// </summary>
    bool Muted { get; }

    /// <summary>
    /// The mute configuration of the channel, or null when there is none.
    /// </summary>
    IMuteConfiguration? MuteConfig { get; }
}

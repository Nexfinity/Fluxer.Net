namespace Fluxer.Net;

/// <summary>
/// The notification and mute settings of a user for one guild.
/// </summary>
public interface IUserGuildSettings
{
    /// <summary>
    /// The settings of individual channels, by channel id, or null when there are none.
    /// </summary>
    IDictionary<ulong, IGuildChannelOverride>? ChannelOverrides { get; }

    /// <summary>
    /// The id of the guild the settings are for, or null when they are not for a guild.
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    /// Whether muted channels are hidden.
    /// </summary>
    bool HideMutedChannels { get; }

    /// <summary>
    /// Which messages notify the user.
    /// </summary>
    NotificationType MessageNotifications { get; }

    /// <summary>
    /// Which messages count towards the unread badge, or null when not set.
    /// </summary>
    NotificationType? UnreadBadges { get; }

    /// <summary>
    /// Whether mobile push notifications are sent.
    /// </summary>
    bool MobilePush { get; }

    /// <summary>
    /// The mute configuration, or null when there is none.
    /// </summary>
    IMuteConfiguration? MuteConfig { get; }

    /// <summary>
    /// Whether the guild is muted.
    /// </summary>
    bool Muted { get; }

    /// <summary>
    /// Whether @everyone mentions are suppressed.
    /// </summary>
    bool SuppressEveryone { get; }

    /// <summary>
    /// Whether role mentions are suppressed.
    /// </summary>
    bool SuppressRoles { get; }

    /// <summary>
    /// The version number of the settings.
    /// </summary>
    int Version { get; }
}

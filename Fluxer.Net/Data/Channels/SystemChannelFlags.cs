namespace Fluxer.Net;

/// <summary>
/// Flags that control which system messages are sent to a guild's system channel.
/// </summary>
[Flags]
public enum SystemChannelFlags : ulong
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Member join notifications are not sent to the system channel.
    /// </summary>
    SuppressJoinNotifications = 1UL << 1,
}

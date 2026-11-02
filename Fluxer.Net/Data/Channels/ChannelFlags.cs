namespace Fluxer.Net;

/// <summary>
/// Flags set on a channel.
/// </summary>
public enum ChannelFlags
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Forum threads that are pinned.
    /// </summary>
    Pinned = 1 << 1,

    /// <summary>
    /// Forum posts must have a tag.
    /// </summary>
    RequireTag = 1 << 4,

    /// <summary>
    /// Hides the media download option in a forum channel.
    /// </summary>
    HideMediaDownloadOption = 1 << 15,
}
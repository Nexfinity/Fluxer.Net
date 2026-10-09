namespace Fluxer.Net;

public enum ChannelFlags
{
    /// <summary>
    /// Forum threads that are pinned.
    /// </summary>
    Pinned = 1 << 1,

    /// <summary>
    /// Forum channel requires a tag.
    /// </summary>
    RequireTag = 1 << 4,

    /// <summary>
    /// Hide media download in forum channel.
    /// </summary>
    HideMediaDownloadOption = 1 << 15,
}
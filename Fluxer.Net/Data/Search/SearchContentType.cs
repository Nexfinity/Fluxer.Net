using Fluxer.Net.Extensions;

namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <summary>
/// The content types a search can filter by. Serialized by <see cref="SearchContentTypeConverter"/>.
/// </summary>
/// <remarks>
/// The members are 0 to 8, not single bits, but the converter checks them with HasFlag, so it writes overlapping types.
/// </remarks>
public enum SearchContentType
{
    /// <summary>
    /// Messages with an image.
    /// </summary>
    Image,

    /// <summary>
    /// Messages with a sound.
    /// </summary>
    Sound,

    /// <summary>
    /// Messages with a video.
    /// </summary>
    Video,

    /// <summary>
    /// Messages with a file.
    /// </summary>
    File,

    /// <summary>
    /// Messages with a sticker.
    /// </summary>
    Sticker,

    /// <summary>
    /// Messages with an embed.
    /// </summary>
    Embed,

    /// <summary>
    /// Messages with a link.
    /// </summary>
    Link,

    /// <summary>
    /// Messages with a poll.
    /// </summary>
    Poll,

    /// <summary>
    /// Messages with a message snapshot.
    /// </summary>
    Snapshot
}

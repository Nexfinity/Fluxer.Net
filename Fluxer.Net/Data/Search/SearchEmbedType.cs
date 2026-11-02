using Fluxer.Net.Extensions;

namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <summary>
/// The embed types a search can filter by. Serialized by <see cref="SearchEmbedTypeConverter"/>.
/// </summary>
/// <remarks>
/// The members are 0 to 3, not single bits. Image is 0 and Article equals Video | Sound, so the converter writes overlapping types.
/// </remarks>
[Flags]
public enum SearchEmbedType
{
    /// <summary>
    /// Embeds with an image.
    /// </summary>
    Image,

    /// <summary>
    /// Embeds with a video.
    /// </summary>
    Video,

    /// <summary>
    /// Embeds with a sound.
    /// </summary>
    Sound,

    /// <summary>
    /// Embeds of an article.
    /// </summary>
    Article
}

namespace Fluxer.Net;

public interface IFavouriteGif
{
    /// <summary>
    /// Unique identifier for the favorite gif.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// ID of the user who owns this favorite gif.
    /// </summary>
    ulong UserId { get; }

    /// <summary>
    /// Display name of the gif.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Tags for categorizing and searching the gif.
    /// </summary>
    string[] Tags { get; }

    /// <summary>
    /// ID of the attachment storing the gif.
    /// </summary>
    ulong AttachmentId { get; }

    /// <summary>
    /// Original filename of the gif.
    /// </summary>
    string Filename { get; }

    /// <summary>
    /// MIME type of the gif file.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// CDN URL to access the meme.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Alternative text description for accessibility.
    /// </summary>
    string? AltText { get; }

    /// <summary>
    /// Hash of the file content for deduplication.
    /// </summary>
    string? ContentHash { get; }

    /// <summary>
    /// Width of the image or video in pixels.
    /// </summary>
    int? Width { get; }

    /// <summary>
    /// Height of the image or video in pixels.
    /// </summary>
    int? Height { get; }

    /// <summary>
    /// Duration of the video in seconds
    /// </summary>
    int? Duration { get; }

    /// <summary>
    /// Whether the gif is a video converted from GIF
    /// </summary>
    bool IsGifVideo { get; }

    /// <summary>
    /// Klipy clip slug if the meme was sourced from Klipy.
    /// </summary>
    string? KlipySlug { get; }

    /// <summary>
    /// Tenor view/- identifier if the meme was sourced from Tenor.
    /// </summary>
    string? TenorSlugId { get; }
}

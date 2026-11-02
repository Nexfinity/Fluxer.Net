namespace Fluxer.Net;

/// <summary>
/// A GIF from a search or the featured list.
/// </summary>
public interface IGif
{
    /// <summary>
    /// The id of the GIF result.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The title/description of the GIF.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// The page URL of the GIF.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Direct URL to the GIF media file.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Proxied URL to the GIF media file.
    /// </summary>
    string ProxySource { get; }

    /// <summary>
    /// Width of the GIF in pixels.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Height of the GIF in pixels.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// The slug used to share and re-resolve this GIF.
    /// </summary>
    string? Slug { get; }

    /// <summary>
    /// The name of the provider that produced this GIF.
    /// </summary>
    string? Provider { get; }

    /// <summary>
    /// Format name to media descriptor map. Formats may be absent.
    /// </summary>
    Dictionary<string, GifMediaFormatJson>? Media { get; }

    /// <summary>
    /// Compact thumbhash placeholder for a low-res preview.
    /// </summary>
    string? Placeholder { get; }
}

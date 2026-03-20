namespace Fluxer.Net.Data.Gifs;

public interface IGif
{
    /// <summary>
    /// The unique Gif result id.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The title/description of the GIF.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// The Gif page URL for the GIF.
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
}

namespace Fluxer.Net;

public interface IGifCategory
{
    /// <summary>
    /// The category search term.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// URL to the category preview image.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Proxied URL to the category preview image.
    /// </summary>
    string ProxySource { get; }
}

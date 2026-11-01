namespace Fluxer.Net;

/// <summary>
/// Builds media host URLs for hashed images.
/// </summary>
internal static class CdnUrl
{
    /// <summary>
    /// Builds a PNG URL for a hashed image. An animated hash asks for its first frame, since the media host
    /// otherwise returns every frame stacked into one tall image.
    /// </summary>
    /// <param name="path">The URL up to and including the hash, without an extension.</param>
    /// <param name="hash">The image hash.</param>
    /// <param name="size">The requested size.</param>
    /// <returns>The URL.</returns>
    internal static string Png(string path, string hash, int size)
        => hash.StartsWith("a_", StringComparison.Ordinal)
            ? $"{path}.png?size={size}&animated=false"
            : $"{path}.png?size={size}";
}

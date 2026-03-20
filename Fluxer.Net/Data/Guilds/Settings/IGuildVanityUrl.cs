namespace Fluxer.Net;

public interface IGuildVanityUrl
{
    /// <summary>
    /// The vanity URL code for the guild.
    /// </summary>
    string? Code { get; }

    /// <summary>
    /// The number of times this vanity URL has been used.
    /// </summary>
    int Uses { get; }
}

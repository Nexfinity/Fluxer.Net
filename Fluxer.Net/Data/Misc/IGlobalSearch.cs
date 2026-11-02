namespace Fluxer.Net;

/// <summary>
/// A page of results from a global search.
/// </summary>
public interface IGlobalSearch
{
    /// <summary>
    /// The messages that matched the search on this page.
    /// </summary>
    IMessage[] Messages { get; }

    /// <summary>
    /// The channels that matched the search on this page.
    /// </summary>
    IChannel[] Channels { get; }

    /// <summary>
    /// The total number of matches.
    /// </summary>
    ulong Total { get; }

    /// <summary>
    /// The number of matches on each page.
    /// </summary>
    int HitsPerPage { get; }

    /// <summary>
    /// The page number of the results.
    /// </summary>
    int Page { get; }
}

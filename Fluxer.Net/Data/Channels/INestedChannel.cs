namespace Fluxer.Net;

/// <summary>
/// A guild channel that can sit under a category.
/// </summary>
public interface INestedChannel : IGuildChannel
{
    /// <summary>
    /// The ID of the category this channel is in, if known.
    /// </summary>
    ulong? CategoryId { get; }
}

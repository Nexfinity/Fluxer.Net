namespace Fluxer.Net;

/// <summary>
/// A thread inside a text or news channel.
/// </summary>
public interface IThreadChannel : ITextChannel
{
    /// <summary>
    /// The kind of thread.
    /// </summary>
    ThreadType ThreadType { get; }

    /// <summary>
    /// Whether the thread is private.
    /// </summary>
    bool IsPrivate { get; }

    /// <summary>
    /// Approximate number of thread members, capped at 50.
    /// </summary>
    int? MemberCount { get; }

    /// <summary>
    /// IDs of the most recently joined members of a forum or media post, newest first.
    /// </summary>
    ulong[]? MemberIdsPreview { get; }

    /// <summary>
    /// Messages in the thread, excluding the starter and deleted messages.
    /// </summary>
    int? MessageCount { get; }

    /// <summary>
    /// Messages ever sent in the thread, never decremented.
    /// </summary>
    int? TotalMessageSent { get; }
}

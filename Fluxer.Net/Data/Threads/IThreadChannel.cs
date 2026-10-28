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
}

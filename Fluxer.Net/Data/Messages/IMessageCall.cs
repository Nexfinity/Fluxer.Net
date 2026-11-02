namespace Fluxer.Net;

/// <summary>
/// A call started from a message.
/// </summary>
public interface IMessageCall
{
    /// <summary>
    /// The user IDs of participants in the call.
    /// </summary>
    HashSet<ulong> Participants { get; }

    /// <summary>
    /// When the call ended, or null when it has not ended.
    /// </summary>
    DateTimeOffset? EndedAt { get; }
}

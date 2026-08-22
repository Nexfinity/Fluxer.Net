namespace Fluxer.Net;

public interface IMessageCall
{
    /// <summary>
    /// The user IDs of participants in the call.
    /// </summary>
    HashSet<ulong> Participants { get; }

    /// <summary>
    /// The ISO 8601 timestamp of when the call ended.
    /// </summary>
    DateTimeOffset? EndedAt { get; }
}

namespace Fluxer.Net;

/// <summary>
/// A user's membership in a thread.
/// </summary>
public interface IThreadMember
{
    /// <summary>
    /// The ID of the user.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The ID of the thread.
    /// </summary>
    ulong ThreadId { get; }

    /// <summary>
    /// When the user joined the thread.
    /// </summary>
    DateTimeOffset JoinedAt { get; }

    /// <summary>
    /// The thread member flags.
    /// </summary>
    int Flags { get; }

    /// <summary>
    /// The guild member of the user, when Fluxer sends it.
    /// </summary>
    IGuildMember? Member { get; }
}

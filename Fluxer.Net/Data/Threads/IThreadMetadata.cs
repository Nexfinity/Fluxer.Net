namespace Fluxer.Net;

/// <summary>
/// The archive and lock state of a thread.
/// </summary>
public interface IThreadMetadata
{
    /// <summary>
    /// When the thread was archived.
    /// </summary>
    DateTimeOffset ArchivedAt { get; }

    /// <summary>
    /// Whether the thread is archived.
    /// </summary>
    bool IsArchived { get; }

    /// <summary>
    /// The time of inactivity after which the thread is archived.
    /// </summary>
    ThreadArchiveDuration AutoArchiveDuration { get; }

    /// <summary>
    /// When the thread was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Whether the thread is locked.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Whether members can invite others to the thread, or null when not set.
    /// </summary>
    bool? IsInvitable { get; }
}

namespace Fluxer.Net;

public interface IThreadMetadata
{
    DateTimeOffset ArchivedAt { get; }

    bool IsArchived { get; }

    ThreadArchiveDuration AutoArchiveDuration { get; }

    DateTimeOffset CreatedAt { get; }

    bool IsLocked { get; }

    bool? IsInvitable { get; }
}

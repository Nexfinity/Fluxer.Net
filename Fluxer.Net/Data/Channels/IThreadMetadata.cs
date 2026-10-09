namespace Fluxer.Net;

public interface IThreadMetadata
{
    DateTimeOffset ArchivedTimestamp { get; }

    bool IsArchived { get; }

    ThreadArchiveDuration AutoArchiveDuration { get; }

    DateTimeOffset CreatedTimestamp { get; }

    bool IsLocked { get; }
}

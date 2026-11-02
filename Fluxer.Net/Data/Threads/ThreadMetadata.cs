namespace Fluxer.Net;

/// <summary>
/// The archive and lock state of a thread.
/// </summary>
public class ThreadMetadata : Entity, IThreadMetadata
{
    /// <inheritdoc />
    public DateTimeOffset ArchivedAt { get; private set; }

    /// <inheritdoc />
    public bool IsArchived { get; private set; }

    /// <inheritdoc />
    public ThreadArchiveDuration AutoArchiveDuration { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public bool IsLocked { get; private set; }

    /// <inheritdoc />
    public bool? IsInvitable { get; private set; }

    internal ThreadMetadata(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates thread metadata from its JSON representation.
    /// </summary>
    /// <returns><see cref="ThreadMetadata"/></returns>
    public static ThreadMetadata Create(FluxerBaseClient client, ThreadMetadataJson json)
    {
        ThreadMetadata data = new ThreadMetadata(client);
        data.Update(json);
        return data;
    }

    internal void Update(ThreadMetadataJson json)
    {
        ArchivedAt = json.ArchivedAt;
        IsArchived = json.IsArchived;
        AutoArchiveDuration = json.AutoArchiveDuration;
        CreatedAt = json.CreatedAt;
        IsLocked = json.IsLocked;
        IsInvitable = json.IsInvitable;
    }
}
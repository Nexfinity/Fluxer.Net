namespace Fluxer.Net;

public class ThreadMetadata : Entity, IThreadMetadata
{
    /// <inheritdoc />
    public DateTimeOffset ArchivedTimestamp { get; private set; }

    /// <inheritdoc />
    public bool IsArchived { get; private set; }

    /// <inheritdoc />
    public ThreadArchiveDuration AutoArchiveDuration { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedTimestamp { get; private set; }

    /// <inheritdoc />
    public bool IsLocked { get; private set; }

    internal ThreadMetadata(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ThreadMetadata object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static ThreadMetadata Create(FluxerBaseClient client, ThreadMetadataJson json)
    {
        ThreadMetadata data = new ThreadMetadata(client);
        data.Update(json);
        return data;
    }

    internal void Update(ThreadMetadataJson json)
    {
        ArchivedTimestamp = json.ArchivedTimestamp;
        IsArchived = json.IsArchived;
        AutoArchiveDuration = json.AutoArchiveDuration;
        CreatedTimestamp = json.CreatedTimestamp;
        IsLocked = json.IsLocked;
    }
}
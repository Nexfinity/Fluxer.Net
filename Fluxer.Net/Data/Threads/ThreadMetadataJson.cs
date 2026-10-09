using Newtonsoft.Json;

namespace Fluxer.Net;

public class ThreadMetadataJson : IThreadMetadata
{
    /// <inheritdoc />
    [JsonProperty("archive_timestamp")]
    public DateTimeOffset ArchivedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("archived")]
    public bool IsArchived { get; set; }

    /// <inheritdoc />
    [JsonProperty("auto_archive_duration")]
    public ThreadArchiveDuration AutoArchiveDuration { get; set; }

    /// <inheritdoc />
    [JsonProperty("create_timestamp")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("locked")]
    public bool IsLocked { get; set; }

    /// <inheritdoc />
    [JsonProperty("invitable")]
    public bool? IsInvitable { get; set; }
}

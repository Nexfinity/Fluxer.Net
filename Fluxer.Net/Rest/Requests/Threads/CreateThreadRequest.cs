using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Base request body for creating a thread.
/// </summary>
public abstract class CreateThreadRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// How long the thread can be inactive before it is archived, in minutes.
    /// </summary>
    [JsonProperty("auto_archive_duration")]
    public int? ArchiveDuration { get; set; }
}
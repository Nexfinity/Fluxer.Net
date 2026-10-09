using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public abstract class CreateThreadRequest : CreateGuildChannelRequest
{
    [JsonProperty("auto_archive_duration")]
    public int? ArchiveDuration { get; set; }
}
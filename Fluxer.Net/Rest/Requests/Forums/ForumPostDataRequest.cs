using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class ForumPostDataRequest
{
    [JsonProperty("thread_ids")]
    public IEnumerable<ulong> ThreadIds { get; set; } = Array.Empty<ulong>();
}

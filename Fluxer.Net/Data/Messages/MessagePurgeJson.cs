using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessagePurgeJson
{
    [JsonProperty("deleted_count")]
    public int DeletedCount { get; set; }
}

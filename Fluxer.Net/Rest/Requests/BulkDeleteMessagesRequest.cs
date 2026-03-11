using Newtonsoft.Json;

namespace Fluxer.Net;

public class BulkDeleteMessagesRequest
{
    [JsonProperty("message_ids")]
    public HashSet<ulong> MessageIds { get; set; }
}

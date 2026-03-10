using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

public class BulkDeleteMessagesRequest
{
    [JsonProperty("message_ids")]
    public HashSet<ulong> MessageIds { get; set; }
}

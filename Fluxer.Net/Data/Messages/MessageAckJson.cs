using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageAckJson
{
    [JsonProperty("manual")]
    public bool Manual { get; set; }

    [JsonProperty("mention_count")]
    public int MentionCount { get; set; }
}

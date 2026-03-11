using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageAck : Entity
{
    [JsonProperty("manual")]
    public bool Manual { get; set; }

    [JsonProperty("mention_count")]
    public int MentionCount { get; set; }
}

using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class MessageAck
{
    [JsonProperty("manual")]
    public bool Manual { get; set; }
    [JsonProperty("mention_count")]
    public int MentionCount { get; set; }
}

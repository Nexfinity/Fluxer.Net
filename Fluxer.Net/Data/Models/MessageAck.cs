using Newtonsoft.Json;

namespace Fluxer.Net.Objects.Models;

public class MessageAck
{
    [JsonProperty("manual")]
    public bool Manual { get; set; }
    [JsonProperty("mention_count")]
    public int MentionCount { get; set; }
}

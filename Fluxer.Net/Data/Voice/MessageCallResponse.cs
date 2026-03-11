using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageCallResponse : Entity
{
    [JsonProperty("participants")]
    public HashSet<ulong> Participants { get; set; }
    [JsonProperty("ended_timestamp")]
    public DateTime? EndedTimestamp { get; set; }
}

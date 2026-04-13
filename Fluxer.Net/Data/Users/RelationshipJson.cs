using Newtonsoft.Json;

namespace Fluxer.Net;

public class RelationshipJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public RelationshipType Type { get; set; }

    [JsonProperty("nickname")]
    public string? Nickname { get; set; }

    [JsonProperty("since")]
    public DateTime? SinceAt { get; set; }
}

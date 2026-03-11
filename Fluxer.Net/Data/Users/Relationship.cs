using Newtonsoft.Json;

namespace Fluxer.Net;

public class Relationship : Entity
{
    [JsonProperty("source_user_id")]
    public ulong SourceUserId { get; set; }

    [JsonProperty("target_user_id")]
    public ulong TargetUserId { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("nickname")]
    public string? Nickname { get; set; }

    [JsonProperty("since")]
    public DateTime? Since { get; set; }
}

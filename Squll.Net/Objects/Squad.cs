using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class Squad
{
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("members")]
    public SquadMember[] Members { get; set; }
    [JsonProperty("spaces")]
    public Space[] Spaces { get; set; }
    [JsonProperty("properties")]
    public SquadProperties Properties { get; set; }
    [JsonProperty("roles")]
    public Role[] Roles { get; set; }
    [JsonProperty("member_count")]
    public int MemberCount { get; set; }
    [JsonProperty("presences")]
    public Presence[] Presences { get; set; }
}

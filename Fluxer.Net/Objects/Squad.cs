using Newtonsoft.Json;

namespace Fluxer.Net.Objects;

public class Community
{
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("members")]
    public CommunityMember[] Members { get; set; }
    [JsonProperty("channels")]
    public Channel[] Channels { get; set; }
    [JsonProperty("properties")]
    public CommunityProperties Properties { get; set; }
    [JsonProperty("roles")]
    public Role[] Roles { get; set; }
    [JsonProperty("member_count")]
    public int MemberCount { get; set; }
    [JsonProperty("presences")]
    public Presence[] Presences { get; set; }
}

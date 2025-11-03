using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class Guild
{
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("members")]
    public GuildMember[] Members { get; set; }
    [JsonProperty("channels")]
    public Channel[] Channels { get; set; }
    [JsonProperty("properties")]
    public GuildProperties Properties { get; set; }
    [JsonProperty("roles")]
    public Role[] Roles { get; set; }
    [JsonProperty("member_count")]
    public int MemberCount { get; set; }
    [JsonProperty("presences")]
    public Presence[] Presences { get; set; }
}

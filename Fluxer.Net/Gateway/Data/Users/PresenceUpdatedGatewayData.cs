using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class PresenceGatewayData
{
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("user")]
    public UserJson User { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("custom_status")]
    public UserCustomStatusJson? CustomStatus { get; set; }

    //Defunct or wip?
    //[JsonProperty("activities")]
    //public List<ActivityJson>? Activities { get; set; }

    //Defunct or wip?
    //[JsonProperty("client_status")]
    //public ClientStatusJson? ClientStatus { get; set; }

    [JsonProperty("afk")]
    public bool Afk { get; set; }

    [JsonProperty("mobile")]
    public bool Mobile { get; set; }
}
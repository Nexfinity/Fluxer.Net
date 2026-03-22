using Newtonsoft.Json;

namespace Fluxer.Net;

public class PresenceJson : IPresence
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("activities")]
    public List<ActivityJson>? Activities { get; set; }

    [JsonProperty("client_status")]
    public ClientStatusJson? ClientStatus { get; set; }

    IEnumerable<IActivity>? IPresence.Activities => Activities;

    IClientStatus? IPresence.ClientStatus => ClientStatus;
}

public class ClientStatusJson : IClientStatus
{
    [JsonProperty("desktop")]
    public string? Desktop { get; set; }

    [JsonProperty("mobile")]
    public string? Mobile { get; set; }

    [JsonProperty("web")]
    public string? Web { get; set; }
}

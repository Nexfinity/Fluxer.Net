using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class PresenceJson : IPresence
{
    /// <inheritdoc />
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <inheritdoc />
    [JsonProperty("activities")]
    public List<ActivityJson>? Activities { get; set; }

    /// <inheritdoc />
    [JsonProperty("client_status")]
    public ClientStatusJson? ClientStatus { get; set; }

    IEnumerable<IActivity>? IPresence.Activities => Activities;

    IClientStatus? IPresence.ClientStatus => ClientStatus;
}

/// <inheritdoc />
public class ClientStatusJson : IClientStatus
{
    /// <inheritdoc />
    [JsonProperty("desktop")]
    public string? Desktop { get; set; }

    /// <inheritdoc />
    [JsonProperty("mobile")]
    public string? Mobile { get; set; }

    /// <inheritdoc />
    [JsonProperty("web")]
    public string? Web { get; set; }
}

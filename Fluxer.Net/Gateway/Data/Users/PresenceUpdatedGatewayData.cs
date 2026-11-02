using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A user's presence, as presence events carry it.
/// </summary>
public class PresenceGatewayData
{
    /// <summary>
    /// The ID of the guild the presence is for, if any.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <summary>
    /// The user the presence belongs to.
    /// </summary>
    [JsonProperty("user")]
    public UserJson User { get; set; }

    /// <summary>
    /// The user's status, such as online or offline.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// The user's custom status, or null when they have none.
    /// </summary>
    [JsonProperty("custom_status")]
    public UserCustomStatusJson? CustomStatus { get; set; }

    //Defunct or wip?
    //[JsonProperty("activities")]
    //public List<ActivityJson>? Activities { get; set; }

    //Defunct or wip?
    //[JsonProperty("client_status")]
    //public ClientStatusJson? ClientStatus { get; set; }

    /// <summary>
    /// Whether the user is away from keyboard.
    /// </summary>
    [JsonProperty("afk")]
    public bool Afk { get; set; }

    /// <summary>
    /// Whether the user is connected from a mobile client.
    /// </summary>
    [JsonProperty("mobile")]
    public bool Mobile { get; set; }
}
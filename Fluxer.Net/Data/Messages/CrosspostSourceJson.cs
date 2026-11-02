using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The source of a message that was crossposted from another channel.
/// </summary>
public class CrosspostSourceJson
{
    /// <summary>
    /// The guild the message was published from.
    /// </summary>
    [JsonProperty("guild")]
    public CrosspostSourceGuildJson Guild { get; set; }
}

/// <summary>
/// The public details of the guild an announcement was published from.
/// </summary>
public class CrosspostSourceGuildJson
{
    /// <summary>
    /// The ID of the guild.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The name of the guild.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The icon hash of the guild, or null when it has no icon.
    /// </summary>
    [JsonProperty("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The banner hash of the guild, or null when it has no banner.
    /// </summary>
    [JsonProperty("banner")]
    public string? Banner { get; set; }

    /// <summary>
    /// The guild's discovery description, or null when it is not listed in discovery.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The features enabled for the guild.
    /// </summary>
    [JsonProperty("features")]
    public string[] Features { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The approximate number of members, or null when it is not sent.
    /// </summary>
    [JsonProperty("approximate_member_count")]
    public int? ApproximateMemberCount { get; set; }

    /// <summary>
    /// The approximate number of online members, or null when it is not sent.
    /// </summary>
    [JsonProperty("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; set; }

    /// <summary>
    /// Whether the guild can be joined through discovery.
    /// </summary>
    [JsonProperty("discoverable")]
    public bool IsDiscoverable { get; set; }
}

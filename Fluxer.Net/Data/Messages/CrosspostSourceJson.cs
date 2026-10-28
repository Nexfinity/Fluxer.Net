using Newtonsoft.Json;

namespace Fluxer.Net;

public class CrosspostSourceJson
{
    [JsonProperty("guild")]
    public CrosspostSourceGuildJson Guild { get; set; }
}

/// <summary>
/// The public details of the guild an announcement was published from.
/// </summary>
public class CrosspostSourceGuildJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

    [JsonProperty("banner")]
    public string? Banner { get; set; }

    /// <summary>
    /// The guild's discovery description, or null when it is not listed in discovery.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("features")]
    public string[] Features { get; set; } = Array.Empty<string>();

    [JsonProperty("approximate_member_count")]
    public int? ApproximateMemberCount { get; set; }

    [JsonProperty("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; set; }

    /// <summary>
    /// Whether the guild can be joined through discovery.
    /// </summary>
    [JsonProperty("discoverable")]
    public bool IsDiscoverable { get; set; }
}

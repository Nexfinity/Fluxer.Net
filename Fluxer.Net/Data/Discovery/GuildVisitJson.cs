using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A guild the current user is visiting without being a member.
/// </summary>
public class GuildVisitJson
{
    /// <summary>
    /// The guild being visited.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// How the visit started: 1 invite, 2 custom invite URL, 3 discovery, 4 the instance community.
    /// </summary>
    [JsonProperty("source")]
    public int Source { get; set; }

    /// <summary>
    /// When the visit started.
    /// </summary>
    [JsonProperty("started_at")]
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// When the visit was last opened, updated at most once a day.
    /// </summary>
    [JsonProperty("opened_at")]
    public DateTimeOffset OpenedAt { get; set; }
}

/// <summary>
/// The result of starting a guild visit.
/// </summary>
public class GuildVisitStartJson
{
    /// <summary>
    /// The guild.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// True when the current user is already a member, in which case no visit starts.
    /// </summary>
    [JsonProperty("member")]
    public bool IsMember { get; set; }

    /// <summary>
    /// The visit, or null when the current user is already a member.
    /// </summary>
    [JsonProperty("visit")]
    public GuildVisitJson? Visit { get; set; }
}

/// <summary>
/// A discoverable guild and one of its channels that new members can view.
/// </summary>
public class DiscoveryChannelPreviewJson
{
    /// <summary>
    /// The discoverable guild the channel belongs to.
    /// </summary>
    [JsonProperty("guild")]
    public DiscoveryChannelPreviewGuildJson Guild { get; set; }

    /// <summary>
    /// The channel.
    /// </summary>
    [JsonProperty("channel")]
    public DiscoveryChannelPreviewChannelJson Channel { get; set; }
}

/// <summary>
/// The guild part of a discovery channel preview.
/// </summary>
public class DiscoveryChannelPreviewGuildJson
{
    /// <summary>
    /// The guild id.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The guild name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The guild icon hash.
    /// </summary>
    [JsonProperty("icon")]
    public string? Icon { get; set; }
}

/// <summary>
/// The channel part of a discovery channel preview.
/// </summary>
public class DiscoveryChannelPreviewChannelJson
{
    /// <summary>
    /// The channel id.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The channel name.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The channel type.
    /// </summary>
    [JsonProperty("type")]
    public int Type { get; set; }
}

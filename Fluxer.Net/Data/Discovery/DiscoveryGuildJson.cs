using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A discovery category.
/// </summary>
public class DiscoveryCategoryJson
{
    /// <summary>
    /// The ID of the category.
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// The name of the category.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }
}

/// <summary>
/// A search result page of discoverable guilds.
/// </summary>
public class DiscoveryGuildListJson
{
    /// <summary>
    /// The guilds matching the search.
    /// </summary>
    [JsonProperty("guilds")]
    public DiscoveryGuildJson[] Guilds { get; set; } = Array.Empty<DiscoveryGuildJson>();

    /// <summary>
    /// The number of guilds matching the search.
    /// </summary>
    [JsonProperty("total")]
    public int Total { get; set; }

    /// <summary>
    /// The number of matches in each category, ignoring the category filter.
    /// </summary>
    [JsonProperty("category_counts")]
    public DiscoveryCategoryCountJson[] CategoryCounts { get; set; } = Array.Empty<DiscoveryCategoryCountJson>();
}

/// <summary>
/// The number of matching guilds in one discovery category.
/// </summary>
public class DiscoveryCategoryCountJson
{
    /// <summary>
    /// The ID of the discovery category.
    /// </summary>
    [JsonProperty("category_type")]
    public int Category { get; set; }

    /// <summary>
    /// The number of matching guilds in the category.
    /// </summary>
    [JsonProperty("count")]
    public int Count { get; set; }
}

/// <summary>
/// A guild as listed in discovery.
/// </summary>
public class DiscoveryGuildJson
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
    /// The icon hash of the guild.
    /// </summary>
    [JsonProperty("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The banner hash of the guild.
    /// </summary>
    [JsonProperty("banner")]
    public string? Banner { get; set; }

    /// <summary>
    /// The description of the guild.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The ID of the discovery category of the guild.
    /// </summary>
    [JsonProperty("category_type")]
    public int Category { get; set; }

    /// <summary>
    /// The primary language of the guild.
    /// </summary>
    [JsonProperty("primary_language")]
    public string? PrimaryLanguage { get; set; }

    /// <summary>
    /// The custom tags of the guild.
    /// </summary>
    [JsonProperty("custom_tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The number of members in the guild.
    /// </summary>
    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    /// <summary>
    /// The number of online members in the guild.
    /// </summary>
    [JsonProperty("online_count")]
    public int OnlineCount { get; set; }

    /// <summary>
    /// The features enabled for the guild.
    /// </summary>
    [JsonProperty("features")]
    public string[] Features { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The verification level of the guild.
    /// </summary>
    [JsonProperty("verification_level")]
    public int VerificationLevel { get; set; }
}

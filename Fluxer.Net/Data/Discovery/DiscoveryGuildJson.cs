using Newtonsoft.Json;

namespace Fluxer.Net;

public class DiscoveryCategoryJson
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class DiscoveryGuildListJson
{
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

public class DiscoveryCategoryCountJson
{
    [JsonProperty("category_type")]
    public int Category { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }
}

public class DiscoveryGuildJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

    [JsonProperty("banner")]
    public string? Banner { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("category_type")]
    public int Category { get; set; }

    [JsonProperty("primary_language")]
    public string? PrimaryLanguage { get; set; }

    [JsonProperty("custom_tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("online_count")]
    public int OnlineCount { get; set; }

    [JsonProperty("features")]
    public string[] Features { get; set; } = Array.Empty<string>();

    [JsonProperty("verification_level")]
    public int VerificationLevel { get; set; }
}

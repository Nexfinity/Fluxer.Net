using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class SquadProperties
{
    [JsonProperty("id")]
    public ulong ID { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("icon")]
    public string? Icon { get; set; }
    [JsonProperty("features")]
    public string[]? Features { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("banner")]
    public string? Banner { get; set; }
    [JsonProperty("splash")]
    public string? Splash { get; set; }
    [JsonProperty("discovery_splash")]
    public string? DiscoverySplash { get; set; }
    [JsonProperty("preferred_locale")]
    public string? PreferredLocale { get; set; }
    [JsonProperty("vanity_url_code")]
    public string? VanityUrl { get; set; }
    [JsonProperty("owner_id")]
    public ulong OwnerId { get; set; }
    [JsonProperty("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }
    [JsonProperty("system_channel_id")]
    public ulong? SystemChannelId { get; set; }
    [JsonProperty("max_members")]
    public int? MaxMembers { get; set; }
    [JsonProperty("premium_type")]
    public int? PremiumType { get; set; }
    [JsonProperty("system_space_flags")]
    public int? SystemSpaceFlags { get; set; }
    // no idea what premium_since is
}

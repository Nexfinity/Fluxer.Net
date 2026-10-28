using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class DiscoveryApplicationRequest
{
    /// <summary>
    /// The description shown in discovery, up to 300 characters. Required when applying.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// A category id from GetDiscoveryCategoriesAsync. Required when applying.
    /// </summary>
    [JsonProperty("category_type")]
    public int? Category { get; set; }

    /// <summary>
    /// The guild's main language as a BCP-47 code, such as en-US.
    /// </summary>
    [JsonProperty("primary_language")]
    public string? PrimaryLanguage { get; set; }

    /// <summary>
    /// Up to 10 tags of up to 30 characters each.
    /// </summary>
    [JsonProperty("custom_tags")]
    public IEnumerable<string>? Tags { get; set; }
}

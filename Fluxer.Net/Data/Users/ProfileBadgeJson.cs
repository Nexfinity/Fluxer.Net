using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

/// <summary>
/// A profile badge as other people see it.
/// </summary>
public class ProfileBadgeJson
{
    /// <summary>
    /// Stable identity of the badge. It equals the kind for built-in badges and is custom:{id} for custom badges.
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Badge type, such as "staff", "partner", "operator_pass" or "custom".
    /// </summary>
    [JsonProperty("kind")]
    public string Kind { get; set; }

    /// <summary>
    /// Full badge description, localized for the viewer.
    /// </summary>
    [JsonProperty("label")]
    public string Label { get; set; }

    /// <summary>
    /// Badge image hash, or null for text badges and badges with an asset URL.
    /// </summary>
    [JsonProperty("asset")]
    public string? Asset { get; set; }

    /// <summary>
    /// Absolute image URL for badges whose image is not a static badge hash.
    /// </summary>
    [JsonProperty("asset_url")]
    public string? AssetUrl { get; set; }

    /// <summary>
    /// Image hash to draw at 20 pixels and smaller.
    /// </summary>
    [JsonProperty("small_asset")]
    public string? SmallAsset { get; set; }

    /// <summary>
    /// Image width divided by height, or null for square badges.
    /// </summary>
    [JsonProperty("aspect_ratio")]
    public double? AspectRatio { get; set; }

    /// <summary>
    /// Short text for the badge, such as #42.
    /// </summary>
    [JsonProperty("caption")]
    public string? Caption { get; set; }

    /// <summary>
    /// How many passes the badge stands for.
    /// </summary>
    [JsonProperty("count")]
    public int? Count { get; set; }

    /// <summary>
    /// Label for screen readers with the full count.
    /// </summary>
    [JsonProperty("accessible_label")]
    public string? AccessibleLabel { get; set; }

    /// <summary>
    /// Absolute URL to open when the badge is clicked.
    /// </summary>
    [JsonProperty("href")]
    public string? Href { get; set; }

    /// <summary>
    /// Content for the richer vertical tooltip.
    /// </summary>
    [JsonProperty("details")]
    public JToken? Details { get; set; }

    /// <summary>
    /// Whether the badge key is hidden from other people. Only sent for your own badges.
    /// </summary>
    [JsonProperty("hidden")]
    public bool? Hidden { get; set; }
}

/// <summary>
/// The badges a profile would show with the given badge settings applied.
/// </summary>
public class ProfileBadgePreviewJson
{
    /// <summary>
    /// The badges other people would see.
    /// </summary>
    [JsonProperty("profile_badges")]
    public ProfileBadgeJson[] ProfileBadges { get; set; }

    /// <summary>
    /// Every badge you hold in display order, with whether the settings hide it.
    /// </summary>
    [JsonProperty("own_badges")]
    public ProfileBadgeJson[] OwnBadges { get; set; }
}

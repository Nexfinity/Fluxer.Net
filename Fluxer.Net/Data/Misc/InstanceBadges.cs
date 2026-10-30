using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A page of the instance's community badge catalog.
/// </summary>
public class InstanceBadges
{
    /// <summary>
    /// The version of the catalog. It changes whenever a badge is created, edited, deleted or moved.
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// The badges on this page.
    /// </summary>
    [JsonProperty("badges")]
    public InstanceBadge[] Badges { get; set; } = Array.Empty<InstanceBadge>();

    /// <summary>
    /// Pass this as <c>after</c> to get the next page, or null on the last page and for a lookup by IDs.
    /// </summary>
    [JsonProperty("next")]
    public string? Next { get; set; }
}

/// <summary>
/// A community badge, as listed in <see cref="PartialGuild.BadgeIds"/>.
/// </summary>
public class InstanceBadge
{
    /// <summary>
    /// The ID of the badge.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The name of the badge.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The text shown under the name when hovering over the badge.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The URL of the badge image.
    /// </summary>
    [JsonProperty("asset_url")]
    public string AssetUrl { get; set; } = string.Empty;

    /// <summary>
    /// The link the badge opens, if any.
    /// </summary>
    [JsonProperty("href")]
    public string? Href { get; set; }

    /// <summary>
    /// The position of the badge in the catalog.
    /// </summary>
    [JsonProperty("position")]
    public int Position { get; set; }
}

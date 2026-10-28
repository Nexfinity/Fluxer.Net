using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A guild's discovery application and whether it can apply.
/// </summary>
public class DiscoveryInfoJson : IDiscoveryInfo
{
    [JsonProperty("application")]
    public DiscoveryApplicationJson? Application { get; set; }

    /// <summary>
    /// Whether the guild meets the requirements to apply.
    /// </summary>
    [JsonProperty("eligible")]
    public bool IsEligible { get; set; }

    [JsonProperty("min_member_count")]
    public int MinMemberCount { get; set; }

    IDiscoveryApplication IDiscoveryInfo.Application => Application;
}

public class DiscoveryApplicationJson : IDiscoveryApplication
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The review status of the application, such as pending, approved or rejected.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("category_type")]
    public int Category { get; set; }

    [JsonProperty("primary_language")]
    public string? PrimaryLanguage { get; set; }

    [JsonProperty("custom_tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [JsonProperty("applied_at")]
    public DateTimeOffset AppliedAt { get; set; }

    [JsonProperty("reviewed_at")]
    public DateTimeOffset? ReviewedAt { get; set; }

    [JsonProperty("review_reason")]
    public string? ReviewReason { get; set; }

    [JsonProperty("removed_at")]
    public DateTimeOffset? RemovedAt { get; set; }

    [JsonProperty("removal_reason")]
    public string? RemovalReason { get; set; }
}

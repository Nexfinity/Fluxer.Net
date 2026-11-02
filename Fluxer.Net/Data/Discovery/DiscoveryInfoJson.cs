using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A guild's discovery application and whether it can apply.
/// </summary>
public class DiscoveryInfoJson : IDiscoveryInfo
{
    /// <inheritdoc />
    [JsonProperty("application")]
    public DiscoveryApplicationJson? Application { get; set; }

    /// <summary>
    /// Whether the guild meets the requirements to apply.
    /// </summary>
    [JsonProperty("eligible")]
    public bool IsEligible { get; set; }

    /// <summary>
    /// The member count a guild needs to apply.
    /// </summary>
    [JsonProperty("min_member_count")]
    public int MinMemberCount { get; set; }

    IDiscoveryApplication IDiscoveryInfo.Application => Application;
}

/// <summary>
/// A guild's application to be listed in discovery.
/// </summary>
public class DiscoveryApplicationJson : IDiscoveryApplication
{
    /// <summary>
    /// The ID of the guild that applied.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The review status of the application, such as pending, approved or rejected.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// The description the guild gave in its application.
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// The ID of the discovery category the guild applied for.
    /// </summary>
    [JsonProperty("category_type")]
    public int Category { get; set; }

    /// <summary>
    /// The primary language the guild gave in its application.
    /// </summary>
    [JsonProperty("primary_language")]
    public string? PrimaryLanguage { get; set; }

    /// <summary>
    /// The custom tags the guild gave in its application.
    /// </summary>
    [JsonProperty("custom_tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The time the application was submitted.
    /// </summary>
    [JsonProperty("applied_at")]
    public DateTimeOffset AppliedAt { get; set; }

    /// <summary>
    /// The time the application was reviewed.
    /// </summary>
    [JsonProperty("reviewed_at")]
    public DateTimeOffset? ReviewedAt { get; set; }

    /// <summary>
    /// The reason given for the review decision.
    /// </summary>
    [JsonProperty("review_reason")]
    public string? ReviewReason { get; set; }

    /// <summary>
    /// The time the guild was removed from discovery.
    /// </summary>
    [JsonProperty("removed_at")]
    public DateTimeOffset? RemovedAt { get; set; }

    /// <summary>
    /// The reason the guild was removed from discovery.
    /// </summary>
    [JsonProperty("removal_reason")]
    public string? RemovalReason { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Badge settings to preview. Settings left out keep their saved values.
/// </summary>
public class ProfileBadgePreviewRequest
{
    /// <summary>
    /// The full list of badge keys to hide from other people.
    /// </summary>
    [JsonProperty("hidden_profile_badges")]
    public IEnumerable<string>? HiddenProfileBadges { get; set; }

    /// <summary>
    /// Mask the premium badge.
    /// </summary>
    [JsonProperty("premium_badge_masked")]
    public bool? PremiumBadgeMasked { get; set; }

    /// <summary>
    /// Hide the premium badge date.
    /// </summary>
    [JsonProperty("premium_badge_timestamp_hidden")]
    public bool? PremiumBadgeTimestampHidden { get; set; }

    /// <summary>
    /// Hide the month and year the Operator Pass was first held.
    /// </summary>
    [JsonProperty("operator_pass_badge_timestamp_hidden")]
    public bool? OperatorPassBadgeTimestampHidden { get; set; }

    /// <summary>
    /// Hide the number of Operator Passes held.
    /// </summary>
    [JsonProperty("operator_pass_badge_count_hidden")]
    public bool? OperatorPassBadgeCountHidden { get; set; }
}

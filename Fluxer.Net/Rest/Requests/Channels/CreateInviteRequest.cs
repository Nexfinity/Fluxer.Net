using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating an invite to a channel.
/// </summary>
public class CreateInviteRequest
{
    /// <summary>
    /// The maximum number of times the invite can be used.
    /// </summary>
    [JsonProperty("max_uses")]
    public int? MaxUses { get; set; }

    /// <summary>
    /// The duration in seconds before the invite expires.
    /// </summary>
    [JsonProperty("max_age")]
    public int? MaxAge { get; set; }

    /// <summary>
    /// Whether to create a unique invite rather than reuse an existing one.
    /// </summary>
    [JsonProperty("unique")]
    public bool? Unique { get; set; }

    /// <summary>
    /// Whether the invite grants temporary membership.
    /// </summary>
    [JsonProperty("temporary")]
    public bool? Temporary { get; set; }
}

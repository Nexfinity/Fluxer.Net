using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A channel's slowmode as it applies to the current user.
/// </summary>
public class SlowmodeStateJson
{
    /// <summary>
    /// The slowmode interval in seconds, or 0 when slowmode is off.
    /// </summary>
    [JsonProperty("rate_limit_per_user")]
    public int Interval { get; set; }

    /// <summary>
    /// How long the current user must wait before sending, in milliseconds.
    /// </summary>
    [JsonProperty("retry_after_ms")]
    public int RetryAfterMs { get; set; }

    [JsonProperty("next_send_allowed_at")]
    public DateTimeOffset? NextSendAllowedAt { get; set; }

    /// <summary>
    /// Whether the current user can ignore slowmode.
    /// </summary>
    [JsonProperty("can_bypass")]
    public bool CanBypass { get; set; }
}

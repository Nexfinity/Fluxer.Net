using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The RATE_LIMITED payload, sent when the gateway rate limits the session.
/// </summary>
public class RateLimitedGatewayData
{
    /// <summary>
    /// The opcode that was rate limited.
    /// </summary>
    [JsonProperty("opcode")]
    public int Opcode { get; set; }

    /// <summary>
    /// How long to wait before retrying, as Fluxer reports it.
    /// </summary>
    [JsonProperty("retry_after")]
    public int RetryAfter { get; set; }

    /// <summary>
    /// Details of the rate limit.
    /// </summary>
    [JsonProperty("meta")]
    public RateLimitedMetaGatewayData Meta { get; set; }
}

/// <summary>
/// The details of a rate limit notice.
/// </summary>
public class RateLimitedMetaGatewayData
{
    /// <summary>
    /// The ID of the guild the rate limit applies to, if any.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}
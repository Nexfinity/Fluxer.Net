using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class AuthSessionJson : IAuthSession
{
    /// <inheritdoc />
    [JsonProperty("session_id_hash")]
    public byte[] SessionIdHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("approx_last_used_at")]
    public DateTimeOffset ApproximateLastUsedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("client_ip")]
    public string ClientIp { get; set; }

    /// <inheritdoc />
    [JsonProperty("client_ip_reverse")]
    public string? ClientIpReverse { get; set; }

    /// <inheritdoc />
    [JsonProperty("client_os")]
    public string? ClientOs { get; set; }

    /// <inheritdoc />
    [JsonProperty("client_platform")]
    public string? ClientPlatform { get; set; }

    /// <inheritdoc />
    [JsonProperty("client_country")]
    public string? ClientCountry { get; set; }
}

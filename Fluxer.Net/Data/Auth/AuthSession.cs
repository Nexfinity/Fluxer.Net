using Newtonsoft.Json;

namespace Fluxer.Net;

public class AuthSession
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("session_id_hash")]
    public byte[] SessionIdHash { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("approx_last_used_at")]
    public DateTime ApproximateLastUsedAt { get; set; }

    [JsonProperty("client_ip")]
    public string ClientIp { get; set; }

    [JsonProperty("client_os")]
    public string ClientOs { get; set; }

    [JsonProperty("client_platform")]
    public string ClientPlatform { get; set; }

    [JsonProperty("client_country")]
    public string? ClientCountry { get; set; }
}

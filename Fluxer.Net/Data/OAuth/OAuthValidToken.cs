using Newtonsoft.Json;

namespace Fluxer.Net;

public class OAuthValidToken : Entity
{
    [JsonProperty("active")]
    public bool IsActive { get; set; }

    [JsonProperty("scope")]
    public string? Scope { get; set; }

    [JsonProperty("client_id")]
    public ulong? ClientId { get; set; }

    [JsonProperty("token_type")]
    public string? TokenType { get; set; }

    [JsonProperty("exp")]
    public int? Exp { get; set; }

    [JsonProperty("iat")]
    public int? Iat { get; set; }

    [JsonProperty("Sub")]
    public ulong? Sub { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net.Objects;

public class TokenRevokeRequest
{
    [JsonProperty("token")]
    public string Token { get; set; }
}

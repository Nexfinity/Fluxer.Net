using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class TokenRevokeRequest
{
    [JsonProperty("token")]
    public string Token { get; set; }
}

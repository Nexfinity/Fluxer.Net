using Newtonsoft.Json;

namespace Fluxer.Net.Objects.Models;

public class TokenRevokeRequest
{
    [JsonProperty("token")]
    public string Token { get; set; }
}

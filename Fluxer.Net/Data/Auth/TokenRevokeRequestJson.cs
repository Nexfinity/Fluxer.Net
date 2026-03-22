using Newtonsoft.Json;

namespace Fluxer.Net;

public class TokenRevokeRequestJson
{
    [JsonProperty("token")]
    public string Token { get; set; }
}

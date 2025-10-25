using Newtonsoft.Json;

namespace Fluxer.Net.Objects;

public class LoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}

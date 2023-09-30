using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class LoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}

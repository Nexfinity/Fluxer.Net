using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class LoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("password")]
    public string Password { get; set; }
}

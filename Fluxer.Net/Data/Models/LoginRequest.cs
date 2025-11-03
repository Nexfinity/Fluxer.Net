using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class LoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("password")]
    public string Password { get; set; }
}

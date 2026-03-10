using Newtonsoft.Json;

namespace Fluxer.Net.Data.Auth;

public class LoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("totp_code")]
    public string? TotpCode { get; set; }
}

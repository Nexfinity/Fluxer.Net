using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The request body for logging in with an email and password.
/// </summary>
public class LoginRequestJson
{
    /// <summary>
    /// The email address of the account.
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }

    /// <summary>
    /// The account password.
    /// </summary>
    [JsonProperty("password")]
    public string Password { get; set; }

    /// <summary>
    /// The one-time code for two-factor authentication, if the account uses it.
    /// </summary>
    [JsonProperty("totp_code")]
    public string? TotpCode { get; set; }
}

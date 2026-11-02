using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A token sent to verify an email address.
/// </summary>
public class EmailVerificationTokenJson
{
    /// <summary>
    /// The verification token.
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <summary>
    /// The id of the user the token belongs to.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The email address to verify.
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }
}

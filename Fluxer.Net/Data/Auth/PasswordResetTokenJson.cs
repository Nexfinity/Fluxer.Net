using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A token sent to reset a password.
/// </summary>
public class PasswordResetTokenJson
{
    /// <summary>
    /// The password reset token.
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <summary>
    /// The id of the user the token belongs to.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The email address the token was sent to.
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }
}

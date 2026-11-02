using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for deleting a guild.
/// </summary>
public class DeleteGuildRequest
{
    /// <summary>
    /// MFA verification code from an authenticator app.
    /// </summary>
    [JsonProperty("mfa_code")]
    public string? MfaCode { get; set; }

    /// <summary>
    /// "totp", "webauthn" or "email_code".
    /// </summary>
    [JsonProperty("mfa_method")]
    public string? MfaMethod { get; set; }

    /// <summary>
    /// Account password for sudo verification.
    /// </summary>
    [JsonProperty("password")]
    public string? Password { get; set; }

    /// <summary>
    /// WebAuthn challenge string.
    /// </summary>
    [JsonProperty("webauthn_challenge")]
    public string? WebAuthnChallenge { get; set; }

    /// <summary>
    /// WebAuthn authentication response.
    /// </summary>
    [JsonProperty("webauthn_response")]
    public string? WebAuthnResponse { get; set; }
}

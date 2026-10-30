using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Sudo verification for actions Fluxer guards on user accounts.
/// </summary>
public class SudoVerificationRequest
{

    /// <summary>
    /// Account password for sudo verification.
    /// </summary>
    [JsonProperty("password")]
    public string? Password { get; set; }

    /// <summary>
    /// "totp", "webauthn" or "email_code".
    /// </summary>
    [JsonProperty("mfa_method")]
    public string? MfaMethod { get; set; }

    /// <summary>
    /// MFA verification code from an authenticator app.
    /// </summary>
    [JsonProperty("mfa_code")]
    public string? MfaCode { get; set; }

    /// <summary>
    /// Ticket from the sudo email code endpoint, sent with the "email_code" method.
    /// </summary>
    [JsonProperty("email_code_ticket")]
    public string? EmailCodeTicket { get; set; }

    /// <summary>
    /// The 6-digit code from the email, sent with the "email_code" method.
    /// </summary>
    [JsonProperty("email_code")]
    public string? EmailCode { get; set; }

    /// <summary>
    /// WebAuthn authentication response.
    /// </summary>
    [JsonProperty("webauthn_response")]
    public object? WebAuthnResponse { get; set; }

    /// <summary>
    /// WebAuthn challenge string.
    /// </summary>
    [JsonProperty("webauthn_challenge")]
    public string? WebAuthnChallenge { get; set; }
}

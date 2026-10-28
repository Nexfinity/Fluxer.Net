using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Sudo verification for actions Fluxer guards on user accounts.
/// </summary>
public class SudoVerificationRequest
{
    [JsonProperty("password")]
    public string? Password { get; set; }

    /// <summary>
    /// "totp" or "webauthn".
    /// </summary>
    [JsonProperty("mfa_method")]
    public string? MfaMethod { get; set; }

    [JsonProperty("mfa_code")]
    public string? MfaCode { get; set; }
}

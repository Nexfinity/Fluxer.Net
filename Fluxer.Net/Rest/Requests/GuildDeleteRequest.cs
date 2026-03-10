using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildDeleteRequest
{
    [JsonProperty("mfa_code")]
    public string? MfaCode { get; set; }
    
    [JsonProperty("mfa_method")]
    public string? MfaMethod { get; set; }
    
    [JsonProperty("password")]
    public string? Password { get; set; }
    
    [JsonProperty("webauthn_challenge")]
    public string? WebAuthnChallenge { get; set; }
    
    [JsonProperty("webauthn_response")]
    public string? WebAuthnResponse { get; set; }
}

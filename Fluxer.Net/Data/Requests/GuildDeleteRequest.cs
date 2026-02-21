using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class GuildDeleteRequest
{
    [JsonPropertyName("mfa_code")]
    public string? MfaCode { get; set; }
    
    [JsonPropertyName("mfa_method")]
    public string? MfaMethod { get; set; }
    
    [JsonPropertyName("password")]
    public string? Password { get; set; }
    
    [JsonPropertyName("webauthn_challenge")]
    public string? WebAuthnChallenge { get; set; }
    
    [JsonPropertyName("webauthn_response")]
    public string? WebAuthnResponse { get; set; }
}

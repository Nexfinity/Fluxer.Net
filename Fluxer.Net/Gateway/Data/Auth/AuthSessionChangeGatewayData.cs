using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for AUTH_SESSION_CHANGE event when an auth session changes.
/// </summary>
public class AuthSessionChangeGatewayData
{
    [JsonProperty("old_auth_session_id_hash")]
    public string OldSessionId { get; set; }

    [JsonProperty("new_auth_session_id_hash")]
    public string NewSessionId { get; set; }

    [JsonProperty("new_token")]
    public string NewToken { get; set; }
}

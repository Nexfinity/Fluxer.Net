using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The AUTH_SESSION_CHANGE payload, with the hashes of the old and new auth session IDs and the new token.
/// </summary>
public class AuthSessionChangeGatewayData
{
    /// <summary>
    /// The hash of the old auth session ID.
    /// </summary>
    [JsonProperty("old_auth_session_id_hash")]
    public string OldSessionId { get; set; }

    /// <summary>
    /// The hash of the new auth session ID.
    /// </summary>
    [JsonProperty("new_auth_session_id_hash")]
    public string NewSessionId { get; set; }

    /// <summary>
    /// The token for the new auth session.
    /// </summary>
    [JsonProperty("new_token")]
    public string NewToken { get; set; }
}
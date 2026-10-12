using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class AuthSessionChangeGatewayData
{
    [JsonProperty("old_auth_session_id_hash")]
    public string OldSessionId { get; set; }

    [JsonProperty("new_auth_session_id_hash")]
    public string NewSessionId { get; set; }

    [JsonProperty("new_token")]
    public string NewToken { get; set; }
}
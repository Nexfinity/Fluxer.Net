using Newtonsoft.Json;
using Squll.Net.Objects;

namespace Squll.Net.Gateway;

public class ReadyGatewayData : IGatewayData
{
    [JsonProperty("members")]
    public SquadMember[] Members { get; set; }
    [JsonProperty("notes")]
    public object[] Notes { get; set; }
    [JsonProperty("private_spaces")]
    public object[] PrivateSpaces { get; set; }
    [JsonProperty("relationships")]
    public object[] Relationships { get; set; }
    [JsonProperty("session_id")]
    public string SessionId { get; set; }
    [JsonProperty("squads")]
    public Squad[] Squads { get; set; }
    [JsonProperty("user")]
    public User User { get; set; }
    [JsonProperty("user_settings")]
    public UserSettings UserSettings { get; set; }
    [JsonProperty("v")]
    public string Version { get; set; }
}

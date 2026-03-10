using Fluxer.Net.Data.Guilds;
using Fluxer.Net.Data.Users;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class ReadyGatewayData : IGatewayData
{
    [JsonProperty("members")]
    public GuildMember[] Members { get; set; }
    [JsonProperty("notes")]
    public Dictionary<string, string> Notes { get; set; }
    [JsonProperty("private_channels")]
    public object[] PrivateChannels { get; set; }
    [JsonProperty("relationships")]
    public object[] Relationships { get; set; }
    [JsonProperty("session_id")]
    public string SessionId { get; set; }
    [JsonProperty("guilds")]
    public Guild[] Guilds { get; set; }
    [JsonProperty("user")]
    public User User { get; set; }
    [JsonProperty("user_settings")]
    public UserSettings UserSettings { get; set; }
    [JsonProperty("v")]
    public string Version { get; set; }
    // [JsonProperty("read_states")]
    // public object[] ReadStates { get; set; }
}

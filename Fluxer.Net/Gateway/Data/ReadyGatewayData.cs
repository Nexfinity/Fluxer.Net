using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class ReadyGatewayData
{
    [JsonProperty("members")]
    public GuildMemberJson[] Members { get; set; }
    [JsonProperty("notes")]
    public Dictionary<string, string> Notes { get; set; }
    [JsonProperty("private_channels")]
    public object[] PrivateChannels { get; set; }
    [JsonProperty("relationships")]
    public object[] Relationships { get; set; }
    [JsonProperty("session_id")]
    public string SessionId { get; set; }
    [JsonProperty("guilds")]
    public GuildJson[] Guilds { get; set; }
    [JsonProperty("user")]
    public UserJson User { get; set; }
    [JsonProperty("user_settings")]
    public UserSettingsJson UserSettings { get; set; }
    [JsonProperty("v")]
    public string Version { get; set; }
    // [JsonProperty("read_states")]
    // public object[] ReadStates { get; set; }
}

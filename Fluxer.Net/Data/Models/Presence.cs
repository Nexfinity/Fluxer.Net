using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class Presence
{
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("last_modified")]
    public ulong LastModified { get; set; }
    [JsonProperty("session_id")]
    public string SessionId { get; set; }
    [JsonProperty("activities")]
    public object[] Activities { get; set; }
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}

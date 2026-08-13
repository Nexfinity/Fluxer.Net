using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class RequestMembersPacket
{
    /// <summary>
    /// The guild ID.
    /// </summary>
    [JsonProperty("guild_id")]
    public string GuildId { get; set; }

    [JsonProperty("query")]
    public string Query { get; set; } = "";

    [JsonProperty("presences")]
    public bool Presences { get; set; } = false;

    [JsonProperty("limit")]
    public int Limit { get; set; } = 0;
}

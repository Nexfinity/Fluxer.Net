using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The data of a request for the members of a guild.
/// </summary>
public class RequestMembersPacket
{
    /// <summary>
    /// The guild ID.
    /// </summary>
    [JsonProperty("guild_id")]
    public string GuildId { get; set; }

    /// <summary>
    /// The query to match members against. Empty by default.
    /// </summary>
    [JsonProperty("query")]
    public string Query { get; set; } = "";

    /// <summary>
    /// Whether presences are sent with the members. False by default.
    /// </summary>
    [JsonProperty("presences")]
    public bool Presences { get; set; } = false;

    /// <summary>
    /// The most members to return. 0 by default.
    /// </summary>
    [JsonProperty("limit")]
    public int Limit { get; set; } = 0;
}

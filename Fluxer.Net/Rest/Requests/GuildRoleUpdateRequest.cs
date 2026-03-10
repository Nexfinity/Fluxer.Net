using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

/// <summary>
/// <c>PATCH /guilds/{guild_id}/roles/{role_id}</c>
/// </summary>
public class GuildRoleUpdateRequest
{
    /// <summary>
    /// The color of the role as an integer
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// Whether the role should be displayed separately in the member list
    /// </summary>
    [JsonProperty("hoist")]
    public bool? Hoist { get; set; }

    /// <summary>
    /// The position of the role in the hoisted member list
    /// </summary>
    [JsonProperty("hoist_position")]
    public int? HoistPosition { get; set; }

    /// <summary>
    /// Whether the role can be mentioned by anyone
    /// </summary>
    [JsonProperty("mentionable")]
    public bool? Mentionable { get; set; }

    /// <summary>
    /// The name of the role (1-100 characters)
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("permissions")]
    public ulong? Permissions { get; set; }
}

using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

/// <summary>
/// <c>PATCH /guilds/{guild_id}/roles/{role_id}</c>
/// </summary>
public class GuildRoleUpdateRequest
{
    /// <summary>
    /// The color of the role as an integer
    /// </summary>
    [JsonPropertyName("color")]
    public int? Color { get; set; }

    /// <summary>
    /// Whether the role should be displayed separately in the member list
    /// </summary>
    [JsonPropertyName("hoist")]
    public bool? Hoist { get; set; }

    /// <summary>
    /// The position of the role in the hoisted member list
    /// </summary>
    [JsonPropertyName("hoist_position")]
    public int? HoistPosition { get; set; }

    /// <summary>
    /// Whether the role can be mentioned by anyone
    /// </summary>
    [JsonPropertyName("mentionable")]
    public bool? Mentionable { get; set; }

    /// <summary>
    /// The name of the role (1-100 characters)
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("permissions")]
    public ulong? Permissions { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net.Data.Requests;

/// <summary>
/// <para>Request body for creating a role in a guild.</para>
/// <c>POST /guilds/{guild_id}/roles</c>
/// </summary>
/// <remarks>
/// <see href="https://docs.fluxer.app/resources/guilds#guildrolecreaterequest"/>
/// </remarks>
public class GuildRoleCreateRequest
{
    /// <summary>
    /// Color as an integer (e.g: 0xff0000 for red)
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// The name of the role (1-100 characters)
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("permissions")]
    public ulong? Permissions { get; set; }
}

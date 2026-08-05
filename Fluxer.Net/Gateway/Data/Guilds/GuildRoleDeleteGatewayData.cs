using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for GUILD_ROLE_DELETE event
/// </summary>
public class GuildRoleDeleteGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("role_id")]
    public ulong RoleId { get; set; }
}

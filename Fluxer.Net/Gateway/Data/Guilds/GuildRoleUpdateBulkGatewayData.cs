using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Guilds;

/// <summary>
/// Gateway data for GUILD_ROLE_UPDATE_BULK event when multiple guild roles are updated.
/// </summary>
public class GuildRoleUpdateBulkGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("roles")]
    public List<RoleJson> Roles { get; set; } = new();
}

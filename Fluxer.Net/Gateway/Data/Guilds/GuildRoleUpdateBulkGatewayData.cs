using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildRoleUpdateBulkGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("roles")]
    public List<RoleJson> Roles { get; set; } = new();
}
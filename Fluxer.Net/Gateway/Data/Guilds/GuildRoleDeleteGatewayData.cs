using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildRoleDeleteGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("role_id")]
    public ulong RoleId { get; set; }
}
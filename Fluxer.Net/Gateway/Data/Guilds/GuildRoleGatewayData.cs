using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildRoleGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("role")]
    public RoleJson Role { get; set; } = null!;
}
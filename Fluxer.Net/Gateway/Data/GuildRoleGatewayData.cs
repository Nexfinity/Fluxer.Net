using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for GUILD_ROLE_CREATE and GUILD_ROLE_UPDATE events
/// </summary>
public class GuildRoleGatewayData : IGatewayData
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("role")]
	public RoleJson Role { get; set; } = null!;
}

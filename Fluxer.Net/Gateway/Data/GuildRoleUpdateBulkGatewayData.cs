using Fluxer.Net.Data.Models;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for GUILD_ROLE_UPDATE_BULK event when multiple guild roles are updated.
/// </summary>
public class GuildRoleUpdateBulkGatewayData : IGatewayData
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("roles")]
	public List<GuildRole> Roles { get; set; } = new();
}

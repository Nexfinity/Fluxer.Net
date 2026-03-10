using Fluxer.Net.Data.Guilds;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for GUILD_EMOJIS_UPDATE event when guild emojis are updated.
/// </summary>
public class GuildEmojisUpdateGatewayData : IGatewayData
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("emojis")]
	public List<GuildEmoji> Emojis { get; set; } = new();
}

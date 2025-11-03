using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class UserGuildFolder
{
	[JsonProperty("folder_id")]
	public int FolderId { get; set; }

	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("color")]
	public int? Color { get; set; }

	[JsonProperty("guild_ids")]
	public List<ulong>? GuildIds { get; set; }
}

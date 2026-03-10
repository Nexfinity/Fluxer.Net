using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildSticker
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("description")]
	public string? Description { get; set; }

	[JsonProperty("format_type")]
	public int FormatType { get; set; }

	[JsonProperty("tags")]
	public List<string>? Tags { get; set; }

	[JsonProperty("creator_id")]
	public ulong CreatorId { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class StickerItem
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("format_type")]
	public int FormatType { get; set; }
}

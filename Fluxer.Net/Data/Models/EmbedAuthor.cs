using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class EmbedAuthor
{
	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("url")]
	public string? Url { get; set; }

	[JsonProperty("icon_url")]
	public string? IconUrl { get; set; }
}

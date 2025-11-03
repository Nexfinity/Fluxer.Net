using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class EmbedFooter
{
	[JsonProperty("text")]
	public string? Text { get; set; }

	[JsonProperty("icon_url")]
	public string? IconUrl { get; set; }
}

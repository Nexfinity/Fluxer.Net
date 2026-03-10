using Newtonsoft.Json;

namespace Fluxer.Net;

public class EmbedProvider
{
	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("url")]
	public string? Url { get; set; }
}

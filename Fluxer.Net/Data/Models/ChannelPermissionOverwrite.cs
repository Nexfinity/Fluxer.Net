using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class ChannelPermissionOverwrite
{
	[JsonProperty("type")]
	public int Type { get; set; }

	[JsonProperty("allow")]
	public ulong Allow { get; set; }

	[JsonProperty("deny")]
	public ulong Deny { get; set; }
}

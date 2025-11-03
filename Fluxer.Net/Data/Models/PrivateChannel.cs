using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class PrivateChannel
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("is_gdm")]
	public bool IsGroupDM { get; set; }
}

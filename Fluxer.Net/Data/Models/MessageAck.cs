using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class MessageAck
{
	[JsonProperty("manual")]
	public bool Manual { get; set; }

	[JsonProperty("mention_count")]
	public int MentionCount { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net.Data.Users;

public class UserNote
{
	[JsonProperty("source_user_id")]
	public ulong SourceUserId { get; set; }

	[JsonProperty("target_user_id")]
	public ulong TargetUserId { get; set; }

	[JsonProperty("note")]
	public string Note { get; set; }
}

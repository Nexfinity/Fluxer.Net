using Newtonsoft.Json;

namespace Fluxer.Net.Data.Users;

public class MfaBackupCode
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("code")]
	public string Code { get; set; }

	[JsonProperty("consumed")]
	public bool Consumed { get; set; }
}

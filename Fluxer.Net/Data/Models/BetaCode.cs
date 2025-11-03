using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class BetaCode
{
	[JsonProperty("code")]
	public string Code { get; set; }

	[JsonProperty("creator_id")]
	public ulong CreatorId { get; set; }

	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonProperty("redeemer_id")]
	public ulong? RedeemerId { get; set; }

	[JsonProperty("redeemed_at")]
	public DateTime? RedeemedAt { get; set; }
}

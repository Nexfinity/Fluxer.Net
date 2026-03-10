using Newtonsoft.Json;

namespace Fluxer.Net;

public class PushSubscription
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("subscription_id")]
	public string SubscriptionId { get; set; }

	[JsonProperty("endpoint")]
	public string Endpoint { get; set; }

	[JsonProperty("p256dh_key")]
	public string P256dhKey { get; set; }

	[JsonProperty("auth_key")]
	public string AuthKey { get; set; }

	[JsonProperty("user_agent")]
	public string? UserAgent { get; set; }
}

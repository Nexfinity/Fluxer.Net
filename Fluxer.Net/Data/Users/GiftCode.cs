using Newtonsoft.Json;

namespace Fluxer.Net.Data.Users;

public class GiftCode
{
	[JsonProperty("code")]
	public string Code { get; set; }

	[JsonProperty("amount_cents")]
	public int AmountCents { get; set; }

	[JsonProperty("duration_months")]
	public int DurationMonths { get; set; }

	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonProperty("created_by_user_id")]
	public ulong CreatedByUserId { get; set; }

	[JsonProperty("redeemed_at")]
	public DateTime? RedeemedAt { get; set; }

	[JsonProperty("redeemed_by_user_id")]
	public ulong? RedeemedByUserId { get; set; }

	[JsonProperty("stripe_payment_intent_id")]
	public string? StripePaymentIntentId { get; set; }

	[JsonProperty("visionary_sequence_number")]
	public int? VisionarySequenceNumber { get; set; }

	[JsonProperty("checkout_session_id")]
	public string? CheckoutSessionId { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net;

public class Payment
{
	[JsonProperty("checkout_session_id")]
	public string CheckoutSessionId { get; set; }

	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("stripe_customer_id")]
	public string? StripeCustomerId { get; set; }

	[JsonProperty("payment_intent_id")]
	public string? PaymentIntentId { get; set; }

	[JsonProperty("subscription_id")]
	public string? SubscriptionId { get; set; }

	[JsonProperty("invoice_id")]
	public string? InvoiceId { get; set; }

	[JsonProperty("price_id")]
	public string? PriceId { get; set; }

	[JsonProperty("product_type")]
	public string? ProductType { get; set; }

	[JsonProperty("amount_cents")]
	public int AmountCents { get; set; }

	[JsonProperty("currency")]
	public string Currency { get; set; }

	[JsonProperty("status")]
	public string Status { get; set; }

	[JsonProperty("is_gift")]
	public bool IsGift { get; set; }

	[JsonProperty("gift_code")]
	public string? GiftCode { get; set; }

	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonProperty("completed_at")]
	public DateTime? CompletedAt { get; set; }
}

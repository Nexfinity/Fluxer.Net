namespace Fluxer.Net;

/// <summary>
/// Represents the type of premium/subscription status for a user.
/// </summary>
public enum PremiumType
{
	/// <summary>
	/// No premium subscription.
	/// </summary>
	None = 0,

	/// <summary>
	/// Active premium subscription (recurring).
	/// </summary>
	Subscription = 1,

	/// <summary>
	/// Lifetime premium access (one-time purchase).
	/// </summary>
	Lifetime = 2,
}

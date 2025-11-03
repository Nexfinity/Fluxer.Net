namespace Fluxer.Net.Data.Enums;

[Flags]
public enum UserFlags : ulong
{
	None = 0,
	Staff = 1UL << 0,
	CtpMember = 1UL << 1,
	Partner = 1UL << 2,
	BugHunter = 1UL << 3,
	HighGlobalRateLimit = 1UL << 33,
	Deleted = 1UL << 34,
	DisabledSuspiciousActivity = 1UL << 35,
	SelfDeleted = 1UL << 36,
	PremiumDiscriminator = 1UL << 37,
	Disabled = 1UL << 38,
	HasSessionStarted = 1UL << 39,
	PremiumBadgeHidden = 1UL << 40,
	PremiumBadgeMasked = 1UL << 41,
	PremiumBadgeTimestampHidden = 1UL << 42,
	PremiumBadgeSequenceHidden = 1UL << 43,
	PremiumPerksSanitized = 1UL << 44,
	PremiumPurchaseDisabled = 1UL << 45,
	PremiumEnabledOverride = 1UL << 46,
	RateLimitBypass = 1UL << 47,
	ReportBanned = 1UL << 48,
	VerifiedNotUnderage = 1UL << 49,
	PendingManualVerification = 1UL << 50,
	HasDismissedPremiumOnboarding = 1UL << 51,
}

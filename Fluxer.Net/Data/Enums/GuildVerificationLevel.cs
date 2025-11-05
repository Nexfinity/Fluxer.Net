namespace Fluxer.Net.Data.Enums;

/// <summary>
/// Represents the verification level required for members to participate in a guild.
/// </summary>
public enum GuildVerificationLevel
{
	/// <summary>
	/// Unrestricted - no verification required.
	/// </summary>
	None = 0,

	/// <summary>
	/// Low - members must have a verified email on their account.
	/// </summary>
	Low = 1,

	/// <summary>
	/// Medium - members must be registered on Fluxer for longer than 5 minutes.
	/// </summary>
	Medium = 2,

	/// <summary>
	/// High - members must be a member of the guild for longer than 10 minutes.
	/// </summary>
	High = 3,

	/// <summary>
	/// Very High - members must have a verified phone number.
	/// </summary>
	VeryHigh = 4,
}

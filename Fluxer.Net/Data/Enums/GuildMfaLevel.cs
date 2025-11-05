namespace Fluxer.Net.Data.Enums;

/// <summary>
/// Represents the multi-factor authentication (MFA) requirement level for a guild.
/// </summary>
public enum GuildMfaLevel
{
	/// <summary>
	/// No MFA requirement for moderator actions.
	/// </summary>
	None = 0,

	/// <summary>
	/// MFA required for moderator actions (elevated security).
	/// </summary>
	Elevated = 1,
}

namespace Fluxer.Net;

/// <summary>
/// The multi-factor authentication (MFA) requirement for moderation actions in a guild.
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

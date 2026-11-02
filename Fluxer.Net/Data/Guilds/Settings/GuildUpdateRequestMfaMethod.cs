namespace Fluxer.Net;

// TODO create convert that converts these values to their expected request values
/// <summary>
/// The MFA method used to confirm a guild update request.
/// </summary>
public enum GuildUpdateRequestMfaMethod
{
    /// <summary>
    /// A time-based one-time password.
    /// </summary>
    Totp,

    /// <summary>
    /// A code sent by SMS.
    /// </summary>
    Sms,

    /// <summary>
    /// A WebAuthn security key or passkey.
    /// </summary>
    WebAuthn
}

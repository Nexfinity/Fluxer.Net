namespace Fluxer.Net;

/// <summary>
/// The methods a user can set up for multi-factor authentication.
/// </summary>
public enum AuthenticatorType
{
    /// <summary>
    /// A time based one-time code.
    /// </summary>
    TOTP = 0,

    /// <summary>
    /// A hardware based passkey.
    /// </summary>
    WebAuthN = 2
}

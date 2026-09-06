namespace Fluxer.Net;

/// <summary>
/// 2FA methods used
/// </summary>
public enum AuthenticatorType
{
    /// <summary>
    /// Time based one-time code.
    /// </summary>
    TOTP = 0,

    /// <summary>
    /// Hardward based passkeys.
    /// </summary>
    WebAuthN = 2
}

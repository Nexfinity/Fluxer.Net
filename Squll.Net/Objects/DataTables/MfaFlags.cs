namespace Squll.Net.Objects;

public enum MfaFlags
{
    None = 0,
    Totp = 1 << 0,
    BackupCodes = 1 << 1,
    Webauthn = 1 << 2,
    YubicoOtp = 1 << 3,
}

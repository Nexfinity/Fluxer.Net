namespace Fluxer.Net.Data.Enums;

// TODO create convert that converts these values to their
// expected request values:
// https://docs.fluxer.app/resources/guilds#guildupdaterequestmfamethod
public enum GuildUpdateRequestMfaMethod
{
    Totp,
    Sms,
    WebAuthn
}

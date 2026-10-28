namespace Fluxer.Net;

/// <summary>
/// Error codes the Fluxer API returns in <see cref="FluxerApiException.ErrorCode"/>.
/// </summary>
public static class FluxerErrorCodes
{
    public const string MissingPermissions = "MISSING_PERMISSIONS";

    public const string MissingAccess = "MISSING_ACCESS";

    public const string CannotSendMessagesToUser = "CANNOT_SEND_MESSAGES_TO_USER";

    public const string RateLimited = "RATE_LIMITED";

    public const string UnknownChannel = "UNKNOWN_CHANNEL";

    public const string UnknownEmoji = "UNKNOWN_EMOJI";

    public const string UnknownGuild = "UNKNOWN_GUILD";

    public const string UnknownInvite = "UNKNOWN_INVITE";

    public const string UnknownMember = "UNKNOWN_MEMBER";

    public const string UnknownMessage = "UNKNOWN_MESSAGE";

    public const string UnknownRole = "UNKNOWN_ROLE";

    public const string UnknownUser = "UNKNOWN_USER";

    public const string UnknownWebhook = "UNKNOWN_WEBHOOK";

    public const string MaxReactions = "MAX_REACTIONS";

    public const string MaxGuildRoles = "MAX_GUILD_ROLES";

    public const string MaxGuildChannels = "MAX_GUILD_CHANNELS";
}

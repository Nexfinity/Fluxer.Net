namespace Fluxer.Net;

/// <summary>
/// Error codes the Fluxer API returns in <see cref="Rest.FluxerApiException.ErrorCode"/>, and the gateway sends in
/// <see cref="Gateway.GatewayErrorGatewayData.Code"/>.
/// </summary>
public static class FluxerErrorCodes
{
    /// <summary>
    /// The caller lacks a permission that the request needs.
    /// </summary>
    public const string MissingPermissions = "MISSING_PERMISSIONS";

    /// <summary>
    /// The caller has no access to the target of the request.
    /// </summary>
    public const string MissingAccess = "MISSING_ACCESS";

    /// <summary>
    /// A message cannot be sent to the user.
    /// </summary>
    public const string CannotSendMessagesToUser = "CANNOT_SEND_MESSAGES_TO_USER";

    /// <summary>
    /// The request was rate limited.
    /// </summary>
    public const string RateLimited = "RATE_LIMITED";

    /// <summary>
    /// The channel does not exist.
    /// </summary>
    public const string UnknownChannel = "UNKNOWN_CHANNEL";

    /// <summary>
    /// The emoji does not exist.
    /// </summary>
    public const string UnknownEmoji = "UNKNOWN_EMOJI";

    /// <summary>
    /// The guild does not exist.
    /// </summary>
    public const string UnknownGuild = "UNKNOWN_GUILD";

    /// <summary>
    /// The invite does not exist.
    /// </summary>
    public const string UnknownInvite = "UNKNOWN_INVITE";

    /// <summary>
    /// The guild member does not exist.
    /// </summary>
    public const string UnknownMember = "UNKNOWN_MEMBER";

    /// <summary>
    /// The message does not exist.
    /// </summary>
    public const string UnknownMessage = "UNKNOWN_MESSAGE";

    /// <summary>
    /// The role does not exist.
    /// </summary>
    public const string UnknownRole = "UNKNOWN_ROLE";

    /// <summary>
    /// The user does not exist.
    /// </summary>
    public const string UnknownUser = "UNKNOWN_USER";

    /// <summary>
    /// The webhook does not exist.
    /// </summary>
    public const string UnknownWebhook = "UNKNOWN_WEBHOOK";

    /// <summary>
    /// The message already has the maximum number of reactions.
    /// </summary>
    public const string MaxReactions = "MAX_REACTIONS";

    /// <summary>
    /// The guild already has the maximum number of roles.
    /// </summary>
    public const string MaxGuildRoles = "MAX_GUILD_ROLES";

    /// <summary>
    /// The guild already has the maximum number of channels.
    /// </summary>
    public const string MaxGuildChannels = "MAX_GUILD_CHANNELS";

    /// <summary>
    /// The voice channel has no room for another user.
    /// </summary>
    public const string VoiceChannelFull = "VOICE_CHANNEL_FULL";

    /// <summary>
    /// The voice connection needs consent for peer-to-peer media first.
    /// </summary>
    public const string VoiceP2PConsentRequired = "VOICE_P2P_CONSENT_REQUIRED";

    /// <summary>
    /// Peer-to-peer media is not available for the voice connection.
    /// </summary>
    public const string VoiceP2PUnavailable = "VOICE_P2P_UNAVAILABLE";
}

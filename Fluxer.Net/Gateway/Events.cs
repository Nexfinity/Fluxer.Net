namespace Fluxer.Net.Gateway;

public static class Events
{
    public static string RelationshipAdded = "RELATIONSHIP_ADD";
    public static string RelationshipUpdated = "RELATIONSHIP_UPDATE";
    public static string RelationshipRemoved = "RELATIONSHIP_REMOVE";
    public static string GuildEmojisUpdate = "GUILD_EMOJIS_UPDATE";
    public static string GuildStickersUpdate = "GUILD_STICKERS_UPDATE";
    public static string GroupUserAdded = "CHANNEL_RECIPIENT_ADD";
    public static string GroupUserRemoved = "CHANNEL_RECIPIENT_REMOVE";
    public static string WebhooksUpdated = "WEBHOOKS_UPDATE";
    public static string InviteCreated = "INVITE_CREATE";
    public static string InviteDeleted = "INVITE_DELETE";
    public static string MemberJoined = "GUILD_MEMBER_ADD";
    public static string MemberUpdated = "GUILD_MEMBER_UPDATE";
    public static string MemberRemoved = "GUILD_MEMBER_REMOVE";
    public static string AuditlogCreated = "GUILD_AUDIT_LOG_ENTRY_CREATE";
    public static string UserBanned = "GUILD_BAN_ADD";
    public static string UserUnbanned = "GUILD_BAN_REMOVE";
    public static string MessageCreated = "MESSAGE_CREATE";
    public static string MessageUpdated = "MESSAGE_UPDATE";
    public static string MessageDeleted = "MESSAGE_DELETE";
    public static string MessageDeletedBulk = "MESSAGE_DELETE_BULK";
    public static string ReactionAdded = "MESSAGE_REACTION_ADD";
    public static string ReactionRemoved = "MESSAGE_REACTION_REMOVE";
    public static string ReactionRemovedAll = "MESSAGE_REACTION_REMOVE_ALL";
    public static string ReactionRemovedEmoji = "MESSAGE_REACTION_REMOVE_EMOJI";
    public static string TypingStarted = "TYPING_START";
    public static string ChannelPinsUpdated = "CHANNEL_PINS_UPDATE";
    public static string VoiceStateUpdated = "VOICE_STATE_UPDATE";
    public static string VoiceServerUpdated = "VOICE_SERVER_UPDATE";
    public static string CallCreated = "CALL_CREATE";
    public static string CallUpdated = "CALL_UPDATE";
    public static string CallDeleted = "CALL_DELETE";
}

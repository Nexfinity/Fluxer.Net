namespace Fluxer.Net.Gateway;

/// <summary>
/// The gateway dispatch names of the events, as Fluxer sends them.
/// </summary>
public static class Events
{
    /// <summary>
    /// The dispatch name of a relationship being added.
    /// </summary>
    public static string RelationshipAdded = "RELATIONSHIP_ADD";
    /// <summary>
    /// The dispatch name of a relationship being updated.
    /// </summary>
    public static string RelationshipUpdated = "RELATIONSHIP_UPDATE";
    /// <summary>
    /// The dispatch name of a relationship being removed.
    /// </summary>
    public static string RelationshipRemoved = "RELATIONSHIP_REMOVE";
    /// <summary>
    /// The dispatch name of a guild's emojis being updated.
    /// </summary>
    public static string GuildEmojisUpdate = "GUILD_EMOJIS_UPDATE";
    /// <summary>
    /// The dispatch name of a guild's stickers being updated.
    /// </summary>
    public static string GuildStickersUpdate = "GUILD_STICKERS_UPDATE";
    /// <summary>
    /// The dispatch name of a user being added to a group channel.
    /// </summary>
    public static string GroupUserAdded = "CHANNEL_RECIPIENT_ADD";
    /// <summary>
    /// The dispatch name of a user being removed from a group channel.
    /// </summary>
    public static string GroupUserRemoved = "CHANNEL_RECIPIENT_REMOVE";
    /// <summary>
    /// The dispatch name of a guild's webhooks being updated.
    /// </summary>
    public static string WebhooksUpdated = "WEBHOOKS_UPDATE";
    /// <summary>
    /// The dispatch name of an invite being created.
    /// </summary>
    public static string InviteCreated = "INVITE_CREATE";
    /// <summary>
    /// The dispatch name of an invite being deleted.
    /// </summary>
    public static string InviteDeleted = "INVITE_DELETE";
    /// <summary>
    /// The dispatch name of a member joining a guild.
    /// </summary>
    public static string MemberJoined = "GUILD_MEMBER_ADD";
    /// <summary>
    /// The dispatch name of a guild member being updated.
    /// </summary>
    public static string MemberUpdated = "GUILD_MEMBER_UPDATE";
    /// <summary>
    /// The dispatch name of a member leaving a guild.
    /// </summary>
    public static string MemberRemoved = "GUILD_MEMBER_REMOVE";
    /// <summary>
    /// The dispatch name of an audit log entry being created.
    /// </summary>
    public static string AuditlogCreated = "GUILD_AUDIT_LOG_ENTRY_CREATE";
    /// <summary>
    /// The dispatch name of a user being banned from a guild.
    /// </summary>
    public static string UserBanned = "GUILD_BAN_ADD";
    /// <summary>
    /// The dispatch name of a user being unbanned from a guild.
    /// </summary>
    public static string UserUnbanned = "GUILD_BAN_REMOVE";
    /// <summary>
    /// The dispatch name of a message being created.
    /// </summary>
    public static string MessageCreated = "MESSAGE_CREATE";
    /// <summary>
    /// The dispatch name of a message being updated.
    /// </summary>
    public static string MessageUpdated = "MESSAGE_UPDATE";
    /// <summary>
    /// The dispatch name of a message being deleted.
    /// </summary>
    public static string MessageDeleted = "MESSAGE_DELETE";
    /// <summary>
    /// The dispatch name of several messages being deleted at once.
    /// </summary>
    public static string MessageDeletedBulk = "MESSAGE_DELETE_BULK";
    /// <summary>
    /// The dispatch name of a reaction being added to a message.
    /// </summary>
    public static string ReactionAdded = "MESSAGE_REACTION_ADD";
    /// <summary>
    /// The dispatch name of a reaction being removed from a message.
    /// </summary>
    public static string ReactionRemoved = "MESSAGE_REACTION_REMOVE";
    /// <summary>
    /// The dispatch name of all reactions being removed from a message.
    /// </summary>
    public static string ReactionRemovedAll = "MESSAGE_REACTION_REMOVE_ALL";
    /// <summary>
    /// The dispatch name of all reactions with one emoji being removed from a message.
    /// </summary>
    public static string ReactionRemovedEmoji = "MESSAGE_REACTION_REMOVE_EMOJI";
    /// <summary>
    /// The dispatch name of a user starting to type in a channel.
    /// </summary>
    public static string TypingStarted = "TYPING_START";
    /// <summary>
    /// The dispatch name of a channel's pinned messages being updated.
    /// </summary>
    public static string ChannelPinsUpdated = "CHANNEL_PINS_UPDATE";
    /// <summary>
    /// The dispatch name of a voice state being updated.
    /// </summary>
    public static string VoiceStateUpdated = "VOICE_STATE_UPDATE";
    /// <summary>
    /// The dispatch name of a voice server being assigned or changed.
    /// </summary>
    public static string VoiceServerUpdated = "VOICE_SERVER_UPDATE";
    /// <summary>
    /// The dispatch name of a call being created.
    /// </summary>
    public static string CallCreated = "CALL_CREATE";
    /// <summary>
    /// The dispatch name of a call being updated.
    /// </summary>
    public static string CallUpdated = "CALL_UPDATE";
    /// <summary>
    /// The dispatch name of a call being deleted.
    /// </summary>
    public static string CallDeleted = "CALL_DELETE";
}

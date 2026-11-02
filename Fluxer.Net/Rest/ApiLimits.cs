namespace Fluxer.Net;

/// <summary>
/// The default message, file, guild and user limits of a Fluxer instance.
/// </summary>
public class ApiLimits
{
    /// <summary>
    /// The maximum number of guilds a premium user can join.
    /// </summary>
    public int MaxGuildsPremium = 200;

    /// <summary>
    /// The maximum number of guilds a user can join.
    /// </summary>
    public int MaxGuilds = 100;

    /// <summary>
    /// The maximum number of channels a guild can have.
    /// </summary>
    public int MaxGuildChannels = 500;

    /// <summary>
    /// The maximum number of channels a category can have.
    /// </summary>
    public int MaxChannelsPerCategory = 50;

    /// <summary>
    /// The minimum bitrate of a voice channel.
    /// </summary>
    public int VoiceChannelBitrateMin = 8000;

    /// <summary>
    /// The maximum bitrate of a voice channel.
    /// </summary>
    public int VoiceChannelBitrateMax = 320000;

    /// <summary>
    /// The maximum user limit of a voice channel.
    /// </summary>
    public int VoiceChannelUserLimitMax = 99;

    /// <summary>
    /// The maximum number of users with a camera on in a voice channel.
    /// </summary>
    public int VoiceChannelCameraUserLimit = 25;

    /// <summary>
    /// The maximum number of connections a user can have in a voice channel.
    /// </summary>
    public int VoiceChannelConnectionLimitMax = 100;

    /// <summary>
    /// The maximum slowmode delay of a channel, in seconds.
    /// </summary>
    public int ChannelRateLimitPerUserMax = 21600;

    /// <summary>
    /// The maximum length of a channel topic.
    /// </summary>
    public int ChannelTopicMaxLength = 1024;

    /// <summary>
    /// The maximum length of a voice region id.
    /// </summary>
    public int RTCRegionIdMaxLength = 64;

    /// <summary>
    /// The maximum number of emojis a guild can have.
    /// </summary>
    public int MaxGuildEmojis = 50;

    /// <summary>
    /// The maximum number of animated emojis a guild can have.
    /// </summary>
    public int MaxGuildEmojisAnimated = 50;

    /// <summary>
    /// The maximum number of emojis a guild can have with <see cref="GuildFeatures.MoreEmojis"/>.
    /// </summary>
    public int MaxGuildEmojisMore = 250;

    /// <summary>
    /// The maximum number of animated emojis a guild can have with <see cref="GuildFeatures.MoreEmojis"/>.
    /// </summary>
    public int MaxGuildEmojisAnimatedMore = 250;

    /// <summary>
    /// The maximum number of stickers a guild can have.
    /// </summary>
    public int MaxStickers = 50;

    /// <summary>
    /// The maximum number of stickers a guild can have with <see cref="GuildFeatures.MoreStickers"/>.
    /// </summary>
    public int MaxGuildStickersMore = 250;

    /// <summary>
    /// The maximum number of invites a guild can have.
    /// </summary>
    public int MaxGuildInvites = 1000;

    /// <summary>
    /// The maximum number of members a guild can have.
    /// </summary>
    public int MaxGuildMembers = 1000000;

    /// <summary>
    /// The maximum number of members a guild can have with <see cref="GuildFeatures.IsLargeGuild"/>.
    /// </summary>
    public int MaxGuildMembersLarge = 10000000;

    /// <summary>
    /// The maximum number of uses a limited invite can be set to.
    /// </summary>
    public int MaxInviteUses = 100;

    /// <summary>
    /// The maximum lifetime of an invite, in seconds.
    /// </summary>
    public int MaxInviteAgeSeconds = 604800;

    /// <summary>
    /// The maximum number of roles a guild can have.
    /// </summary>
    public int MaxGuildRoles = 250;

    /// <summary>
    /// The maximum number of apps a user can create.
    /// </summary>
    public int MaxAppsPerUser = 25;

    /// <summary>
    /// The maximum number of webhooks a channel can have.
    /// </summary>
    public int MaxWebhooksPerChannel = 15;

    /// <summary>
    /// The maximum number of webhooks a guild can have.
    /// </summary>
    public int MaxWebhooksPerGuild = 1000;

    /// <summary>
    /// The maximum length of message content for a premium user.
    /// </summary>
    public int MaxMessageContentLengthPremium = 4000;

    /// <summary>
    /// The maximum length of message content.
    /// </summary>
    public int MaxMessageContentLength = 2000;

    /// <summary>
    /// The maximum number of attachments in one message.
    /// </summary>
    public int MaxAttachmentsPerMessage = 10;

    /// <summary>
    /// The maximum number of embeds in one message.
    /// </summary>
    public int MaxMessageEmbeds = 10;

    /// <summary>
    /// The maximum number of reactions a message can have.
    /// </summary>
    public int MaxReactionsPerMessage = 30;

    /// <summary>
    /// The maximum number of users that can react to a message with one emoji.
    /// </summary>
    public int MaxUsersPerMessageReaction = 1000000;

    /// <summary>
    /// The maximum length of attachment alternative text.
    /// </summary>
    public int NaxAttachmentAltTextLength = 4096;


    #region User Limits

    /// <summary>
    /// The maximum length of a profile bio.
    /// </summary>
    public int MaxBioLength = 320;

    /// <summary>
    /// The maximum length of a profile bio for a premium user.
    /// </summary>
    public int MaxBioLengthPremium = 320;

    internal static int AssetMaxBytes = 10 * 1024 * 1024;

    /// <summary>
    /// The maximum size in bytes of an avatar upload.
    /// </summary>
    public int AvatarMaxSize = AssetMaxBytes;

    /// <summary>
    /// The maximum number of relationships a user can have, such as friends.
    /// </summary>
    public int MaxRelationships = 1000;

    /// <summary>
    /// The maximum number of users in a group.
    /// </summary>
    public int MaxGroupMembers = 50;

    /// <summary>
    /// The maximum number of private channels a user can have open.
    /// </summary>
    public int MaxPrivateChannelsPerUser = 250;

    /// <summary>
    /// The maximum number of groups a user can create.
    /// </summary>
    public int MaxGroupsPerUser = 150;

    /// <summary>
    /// The maximum number of bookmarks a premium user can have.
    /// </summary>
    public int MaxBookmarksPremium = 300;

    /// <summary>
    /// The maximum number of bookmarks a user can have.
    /// </summary>
    public int MaxBookmarks = 50;

    /// <summary>
    /// The maximum number of favorite media items a premium user can have.
    /// </summary>
    public int MaxFavoriteMediaPremium = 500;

    /// <summary>
    /// The maximum number of favorite media items a user can have.
    /// </summary>
    public int MaxFavoriteMedia = 50;

    /// <summary>
    /// The maximum number of tags a favorite media item can have.
    /// </summary>
    public int MaxFavoriteMediaTags = 10;

    /// <summary>
    /// The maximum number of favorite media items that can be saved.
    /// </summary>
    public int MaxFavoriteMediaSaved = 10000;

    #endregion

    /// <summary>
    /// The minimum length of an attachment filename.
    /// </summary>
    public const int FilenameTypeMinLength = 1;

    /// <summary>
    /// The maximum length of an attachment filename.
    /// </summary>
    public const int FilenameTypeMaxLength = 255;

    /// <summary>
    /// The minimum length of an embed URL.
    /// </summary>
    public const int UrlTypeMinLength = 1;

    /// <summary>
    /// The maximum length of an embed URL.
    /// </summary>
    public const int UrlTypeMaxLength = 2048;

    /// <summary>
    /// The minimum length of a message nonce.
    /// </summary>
    public const int MessageNonceMinLength = 1;

    /// <summary>
    /// The maximum length of a message nonce.
    /// </summary>
    public const int MessageNonceMaxLength = 32;
}

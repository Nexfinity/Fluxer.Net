namespace Fluxer.Net.Rest;

/// <summary>
/// API limits for the current instance.
/// </summary>
public class ApiLimits
{
    /// <summary>
    /// How many guilds you can join with premium.
    /// </summary>
    public int MaxGuildsPremium = 200;

    /// <summary>
    /// How many guilds you can join.
    /// </summary>
    public int MaxGuilds = 100;

    /// <summary>
    /// How many channels a guild can have.
    /// </summary>
    public int MaxGuildChannels = 500;

    /// <summary>
    /// How many channels a category can have.
    /// </summary>
    public int MaxChannelsPerCategory = 50;

    /// <summary>
    /// Minimum bitrate for voice channel.
    /// </summary>
    public int VoiceChannelBitrateMin = 8000;

    /// <summary>
    /// Maximum bitrate for voice channel.
    /// </summary>
    public int VoiceChannelBitrateMax = 320000;

    /// <summary>
    /// Maximum users that can join a voice channel.
    /// </summary>
    public int VoiceChannelUserLimitMax = 99;

    /// <summary>
    /// Maximum users that can use camera in a voice channel.
    /// </summary>
    public int VoiceChannelCameraUserLimit = 25;

    /// <summary>
    /// Maximum connections a user can have in a voice channel.
    /// </summary>
    public int VoiceChannelConnectionLimitMax = 100;

    public int ChannelRateLimitPerUserMax = 21600;

    /// <summary>
    /// Maximum length a channel topic can have.
    /// </summary>
    public int ChannelTopicMaxLength = 1024;

    /// <summary>
    /// Maximum length a region id can have.
    /// </summary>
    public int RTCRegionIdMaxLength = 64;

    /// <summary>
    /// How many emojis you can create in a guild.
    /// </summary>
    public int MaxGuildEmojis = 50;

    /// <summary>
    /// How many animated emojis you can create in a guild.
    /// </summary>
    public int MaxGuildEmojisAnimated = 50;

    /// <summary>
    /// How many more emojis you can create in a guild with <see cref="GuildFeatures.HasMoreEmojis"/>.
    /// </summary>
    public int MaxGuildEmojisMore = 250;

    /// <summary>
    /// How many more animated emojis you can create in a guild with <see cref="GuildFeatures.HasMoreEmojis"/>.
    /// </summary>
    public int MaxGuildEmojisAnimatedMore = 250;

    /// <summary>
    /// How many stickers you can create in a guild.
    /// </summary>
    public int MaxStickers = 50;

    /// <summary>
    /// How many more stickers you can create in a guild with <see cref="GuildFeatures.HasMoreStickers"/>.
    /// </summary>
    public int MaxGuildStickersMore = 250;

    /// <summary>
    /// Maximum invites a guild can create.
    /// </summary>
    public int MaxGuildInvites = 1000;

    /// <summary>
    /// How many users can join a guild.
    /// </summary>
    public int MaxGuildMembers = 1000000;

    /// <summary>
    /// How many more users can join a guild with <see cref="GuildFeatures.IsLargeServer"/>.
    /// </summary>
    public int MaxGuildMembersLarge = 10000000;

    /// <summary>
    /// Maximum limit a limited invite can be set to.
    /// </summary>
    public int MaxInviteUses = 100;

    /// <summary>
    /// Maximum seconds a limited invite can be set to.
    /// </summary>
    public int MaxInviteAgeSeconds = 604800;

    /// <summary>
    /// Maximum roles a guild can create.
    /// </summary>
    public int MaxGuildRoles = 250;

    /// <summary>
    /// Maximum apps a user can create.
    /// </summary>
    public int MaxAppsPerUser = 25;

    /// <summary>
    /// Maximum webhooks a channel can have.
    /// </summary>
    public int MaxWebhooksPerChannel = 15;

    /// <summary>
    /// Maximum webhooks a guild can create.
    /// </summary>
    public int MaxWebhooksPerGuild = 1000;

    /// <summary>
    /// Max length of message text you can send with premium.
    /// </summary>
    public int MaxMessageContentLengthPremium = 4000;

    /// <summary>
    /// Max length of message text you can send.
    /// </summary>
    public int MaxMessageContentLength = 2000;

    /// <summary>
    /// Max attachments you can send in 1 message.
    /// </summary>
    public int MaxAttachmentsPerMessage = 10;

    /// <summary>
    /// Max embeds you can send in 1 message.
    /// </summary>
    public int MaxMessageEmbeds = 10;

    /// <summary>
    /// Maximum reactions a message can have.
    /// </summary>
    public int MaxReactionsPerMessage = 30;

    /// <summary>
    /// Maximum users each reaction can have.
    /// </summary>
    public int MaxUsersPerMessageReaction = 1000000;

    /// <summary>
    /// Maximum length of attachment alternative text.
    /// </summary>
    public int NaxAttachmentAltTextLength = 4096;


    #region User Limits

    /// <summary>
    /// Maximum length of profile bio/text you can set.
    /// </summary>
    public int MaxBioLength = 320;

    /// <summary>
    /// Maximum length of profile bio/text you can set with premium.
    /// </summary>
    public int MaxBioLengthPremium = 320;

    internal static int AssetMaxBytes = 10 * 1024 * 1024;

    /// <summary>
    /// Maximum size and avatar upload can be.
    /// </summary>
    public int AvatarMaxSize = AssetMaxBytes;

    /// <summary>
    /// Maximum friends a user can have.
    /// </summary>
    public int MaxRelationships = 1000;

    /// <summary>
    /// Maximum users a group can have.
    /// </summary>
    public int MaxGroupMembers = 50;

    /// <summary>
    /// Maximum private channels a user can have open.
    /// </summary>
    public int MaxPrivateChannelsPerUser = 250;

    /// <summary>
    /// Maximum groups a user can create.
    /// </summary>
    public int MaxGroupsPerUser = 150;

    /// <summary>
    /// Max bookmarks you can have with premium.
    /// </summary>
    public int MaxBookmarksPremium = 300;

    /// <summary>
    /// Max bookmarks you can have.
    /// </summary>
    public int MaxBookmarks = 50;

    /// <summary>
    /// Max favorite gifs you can have with premium.
    /// </summary>
    public int MaxFavoriteMemesPremium = 500;

    /// <summary>
    /// Max favorite gifs you can have.
    /// </summary>
    public int MaxFavoriteMemes = 50;

    /// <summary>
    /// Max tags a favorite gif can have.
    /// </summary>
    public int MaxFavoriteMemeTags = 10;

    /// <summary>
    /// Max favorite gifs saved.
    /// </summary>
    public int MaxFavoriteGifs = 10000;

    #endregion

    /// <summary>
    /// Min length of attachment filename.
    /// </summary>
    public const int FilenameTypeMinLength = 1;

    /// <summary>
    /// Max length of attachment filename.
    /// </summary>
    public const int FilenameTypeMaxLength = 255;

    /// <summary>
    /// Min length of embed url.
    /// </summary>
    public const int UrlTypeMinLength = 1;

    /// <summary>
    /// Max length of embed url.
    /// </summary>
    public const int UrlTypeMaxLength = 2048;

    /// <summary>
    /// Min length of message nonce.
    /// </summary>
    public const int MessageNonceMinLength = 1;

    /// <summary>
    /// Max length of message nonce.
    /// </summary>
    public const int MessageNonceMaxLength = 32;
}

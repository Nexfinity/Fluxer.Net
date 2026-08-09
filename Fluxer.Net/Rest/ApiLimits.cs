namespace Fluxer.Net;

/// <summary>
/// API limits for the current instance.
/// </summary>
public class ApiLimits
{
    /// <summary>
    /// How many guilds you can join.
    /// </summary>
    public int MaxGuilds = 100;

    /// <summary>
    /// How many guilds you can join with premium.
    /// </summary>
    public int MaxGuildsPremium = 200;

    /// <summary>
    /// How many emojis you can create in a guild.
    /// </summary>
    public int MaxEmojis = 50;

    /// <summary>
    /// How many animated emojis you can create in a guild.
    /// </summary>
    public int MaxEmojisAnimated = 50;

    /// <summary>
    /// How many more emojis you can create in a guild with <see cref="GuildFeatures.HasMoreEmojis"/>.
    /// </summary>
    public int MaxEmojisMore = 250;

    /// <summary>
    /// How many more animated emojis you can create in a guild with <see cref="GuildFeatures.HasMoreEmojis"/>.
    /// </summary>
    public int MaxEmojisAnimatedMore = 250;

    /// <summary>
    /// How many stickers you can create in a guild.
    /// </summary>
    public int MaxStickers = 50;

    /// <summary>
    /// How many more stickers you can create in a guild with <see cref="GuildFeatures.HasMoreStickers"/>.
    /// </summary>
    public int MaxStickersMore = 250;

    /// <summary>
    /// How many users can join a guild.
    /// </summary>
    public int MaxGuildMembers = 1000;

    /// <summary>
    /// How many more users can join a guild with <see cref="GuildFeatures.IsLargeServer"/>.
    /// </summary>
    public int MaxGuildMembersLarge = 10000;

    /// <summary>
    /// How many channels a category can have.
    /// </summary>
    public int MaxChannelsPerCategory = 50;

    /// <summary>
    /// Max length of message text you can send.
    /// </summary>
    public int MaxMessageContentLength = 2000;

    /// <summary>
    /// Max length of message text you can send with premium.
    /// </summary>
    public int MaxMessageContentLengthPremium = 4000;

    /// <summary>
    /// Max attachments you can send in 1 message.
    /// </summary>
    public int MaxMessageAttachments = 10;

    /// <summary>
    /// Max embeds you can send in 1 message.
    /// </summary>
    public int MaxMessageEmbeds = 10;

    /// <summary>
    /// Max bookmarks you can have.
    /// </summary>
    public int MaxBookmarks = 50;

    /// <summary>
    /// Max bookmarks you can have with premium.
    /// </summary>
    public int MaxBookmarksPremium = 300;

    /// <summary>
    /// Max favorite gifs you can have.
    /// </summary>
    public int MaxFavoriteMemes = 50;

    /// <summary>
    /// Max favorite gifs you can have with premium.
    /// </summary>
    public int MaxFavoriteMemesPremium = 500;

    /// <summary>
    /// Max tags a favorite gif can have.
    /// </summary>
    public int MaxFavoriteMemeTags = 10;

    /// <summary>
    /// Max length of profile bio/text you can set.
    /// </summary>
    public int MaxBioLength = 160;

    /// <summary>
    /// Max length of profile bio/text you can set with premium.
    /// </summary>
    public int MaxBioLengthPremium = 320;

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

namespace Fluxer.Net;

public class ApiLimits
{
    public int MaxGuilds = 100;
    public int MaxGuildsPremium = 200;

    public int MaxEmojis = 50;
    public int MaxEmojisAnimated = 50;
    public int MaxEmojisMore = 250;
    public int MaxEmojisAnimatedMore = 250;
    public int MaxStickers = 50;
    public int MaxStickersMore = 250;

    public int MaxGuildMembers = 1000;
    public int MaxGuildMembersLarge = 10000;
    public int MaxChannelsPerCategory = 50;

    public int MaxMessageContentLength = 2000;
    public int MaxMessageContentLengthPremium = 4000;
    public int MaxMessageAttachments = 10;
    public int MaxMessageEmbeds = 10;

    public int MaxBookmarks = 50;
    public int MaxBookmarksPremium = 300;

    public int MaxFavoriteMemes = 50;
    public int MaxFavoriteMemesPremium = 500;
    public int MaxFavoriteMemeTags = 10;

    public int MaxBioLength = 160;
    public int MaxBioLengthPremium = 320;

    public const int FilenameTypeMinLength = 1;
    public const int FilenameTypeMaxLength = 255;

    public const int UrlTypeMinLength = 1;
    public const int UrlTypeMaxLength = 2048;

    public const int MessageNonceMinLength = 1;
    public const int MessageNonceMaxLength = 32;
}

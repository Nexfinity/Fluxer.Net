namespace Fluxer.Net;

public interface IUserSettings
{
    ulong UserId { get; }

    string Locale { get; }

    string Theme { get; }

    string Status { get; }

    UserCustomStatusJson? CustomStatus { get; }

    bool DeveloperMode { get; }

    bool CompactMessageDisplay { get; }

    bool AnimateEmoji { get; }

    int AnimateStickers { get; }

    bool GifAutoPlay { get; }

    bool RenderEmbeds { get; }

    bool RenderReactions { get; }

    int RenderSpoilers { get; }

    bool InlineAttachmentMedia { get; }

    bool InlineEmbedMedia { get; }

    int ExplicitContentFilter { get; }

    int FriendSourceFlags { get; }

    int IncomingCallFlags { get; }

    int GroupDmAddPermissionFlags { get; }

    bool DefaultGuildsRestricted { get; }

    List<ulong>? RestrictedGuilds { get; }

    List<ulong>? GuildPositions { get; }

    List<UserGuildFolder>? GuildFolders { get; }

    int AfkTimeout { get; }

    int TimeFormat { get; }
}

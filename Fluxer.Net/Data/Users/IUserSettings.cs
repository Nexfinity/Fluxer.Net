namespace Fluxer.Net;

public interface IUserSettings
{
    ulong UserId { get; }

    /// <summary>
    /// The locale code for the user interface language.
    /// </summary>
    string Locale { get; }

    /// <summary>
    /// The UI theme preference
    /// </summary>
    string Theme { get; }

    /// <summary>
    /// The current online status of the user
    /// </summary>
    string Status { get; }

    /// <summary>
    /// The custom status set by the user.
    /// </summary>
    UserCustomStatusJson? CustomStatus { get; }

    /// <summary>
    /// Whether developer mode is enabled.
    /// </summary>
    bool DeveloperMode { get; }

    /// <summary>
    /// Whether to use compact message display mode.
    /// </summary>
    bool CompactMessageDisplay { get; }

    /// <summary>
    /// Whether to animate custom emojis.
    /// </summary>
    bool AnimateEmoji { get; }

    /// <summary>
    /// Sticker animation preference setting.
    /// </summary>
    int AnimateStickers { get; }

    /// <summary>
    /// Whether GIFs auto-play in chat.
    /// </summary>
    bool GifAutoPlay { get; }

    /// <summary>
    /// Whether to render message embeds.
    /// </summary>
    bool RenderEmbeds { get; }

    /// <summary>
    /// Whether to display reactions on messages.
    /// </summary>
    bool RenderReactions { get; }

    /// <summary>
    /// Spoiler rendering preference setting.
    /// </summary>
    int RenderSpoilers { get; }

    /// <summary>
    /// Whether to display attachments inline in chat.
    /// </summary>
    bool InlineAttachmentMedia { get; }

    /// <summary>
    /// Whether to display embed media inline in chat.
    /// </summary>
    bool InlineEmbedMedia { get; }

    int ExplicitContentFilter { get; }

    /// <summary>
    /// Friend source flags.
    /// </summary>
    int FriendSourceFlags { get; }

    /// <summary>
    /// Incoming call settings.
    /// </summary>
    int IncomingCallFlags { get; }

    /// <summary>
    /// Group DM add permissions.
    /// </summary>
    int GroupDmAddPermissionFlags { get; }

    /// <summary>
    /// Whether new guilds have DM restrictions by default.
    /// </summary>
    bool DefaultGuildsRestricted { get; }

    /// <summary>
    /// Guild IDs where direct messages are restricted
    /// </summary>
    List<ulong>? RestrictedGuilds { get; }

    List<ulong>? GuildPositions { get; }

    /// <summary>
    /// The folder structure for organizing guilds in the sidebar.
    /// </summary>
    List<UserGuildFolderJson>? GuildFolders { get; }

    /// <summary>
    /// The idle timeout in seconds before going AFK.
    /// </summary>
    int AfkTimeout { get; }

    /// <summary>
    /// The preferred time format setting.
    /// </summary>
    int TimeFormat { get; }
}

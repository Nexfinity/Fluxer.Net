namespace Fluxer.Net;

/// <summary>
/// The settings of a user, such as language, theme and display options.
/// </summary>
public interface IUserSettings
{
    /// <summary>
    /// The id of the user.
    /// </summary>
    ulong UserId { get; }

    /// <summary>
    /// The locale code for the user interface language.
    /// </summary>
    string Locale { get; }

    /// <summary>
    /// The UI theme preference.
    /// </summary>
    string Theme { get; }

    /// <summary>
    /// The current online status of the user.
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

    /// <summary>
    /// The explicit content filter setting, as the API sends it.
    /// </summary>
    int ExplicitContentFilter { get; }

    /// <summary>
    /// The flags of the sources that can send friend requests to the user.
    /// </summary>
    int FriendSourceFlags { get; }

    /// <summary>
    /// The flags for incoming calls.
    /// </summary>
    int IncomingCallFlags { get; }

    /// <summary>
    /// The flags for who can add the user to group direct messages.
    /// </summary>
    int GroupDmAddPermissionFlags { get; }

    /// <summary>
    /// Whether new guilds have DM restrictions by default.
    /// </summary>
    bool DefaultGuildsRestricted { get; }

    /// <summary>
    /// The ids of the guilds where direct messages are restricted.
    /// </summary>
    List<ulong>? RestrictedGuilds { get; }

    /// <summary>
    /// The ids of the guilds in the order they appear in the sidebar.
    /// </summary>
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

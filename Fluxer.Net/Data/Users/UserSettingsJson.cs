using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class UserSettingsJson : IUserSettings
{
    /// <inheritdoc />
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <inheritdoc />
    [JsonProperty("locale")]
    public string Locale { get; set; }

    /// <inheritdoc />
    [JsonProperty("theme")]
    public string Theme { get; set; }

    /// <inheritdoc />
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <inheritdoc />
    [JsonProperty("custom_status")]
    public UserCustomStatusJson? CustomStatus { get; set; }

    /// <inheritdoc />
    [JsonProperty("developer_mode")]
    public bool DeveloperMode { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_display_compact")]
    public bool CompactMessageDisplay { get; set; }

    /// <inheritdoc />
    [JsonProperty("animate_emoji")]
    public bool AnimateEmoji { get; set; }

    /// <inheritdoc />
    [JsonProperty("animate_stickers")]
    public int AnimateStickers { get; set; }

    /// <inheritdoc />
    [JsonProperty("gif_auto_play")]
    public bool GifAutoPlay { get; set; }

    /// <inheritdoc />
    [JsonProperty("render_embeds")]
    public bool RenderEmbeds { get; set; }

    /// <inheritdoc />
    [JsonProperty("render_reactions")]
    public bool RenderReactions { get; set; }

    /// <inheritdoc />
    [JsonProperty("render_spoilers")]
    public int RenderSpoilers { get; set; }

    /// <inheritdoc />
    [JsonProperty("inline_attachment_media")]
    public bool InlineAttachmentMedia { get; set; }

    /// <inheritdoc />
    [JsonProperty("inline_embed_media")]
    public bool InlineEmbedMedia { get; set; }

    /// <inheritdoc />
    [JsonProperty("explicit_content_filter")]
    public int ExplicitContentFilter { get; set; }

    /// <inheritdoc />
    [JsonProperty("friend_source_flags")]
    public int FriendSourceFlags { get; set; }

    /// <inheritdoc />
    [JsonProperty("incoming_call_flags")]
    public int IncomingCallFlags { get; set; }

    /// <inheritdoc />
    [JsonProperty("group_dm_add_permission_flags")]
    public int GroupDmAddPermissionFlags { get; set; }

    /// <inheritdoc />
    [JsonProperty("default_guilds_restricted")]
    public bool DefaultGuildsRestricted { get; set; }

    /// <inheritdoc />
    [JsonProperty("restricted_guilds")]
    public List<ulong>? RestrictedGuilds { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_positions")]
    public List<ulong>? GuildPositions { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_folders")]
    public List<UserGuildFolderJson>? GuildFolders { get; set; }

    /// <inheritdoc />
    [JsonProperty("afk_timeout")]
    public int AfkTimeout { get; set; }

    /// <inheritdoc />
    [JsonProperty("time_format")]
    public int TimeFormat { get; set; }
}

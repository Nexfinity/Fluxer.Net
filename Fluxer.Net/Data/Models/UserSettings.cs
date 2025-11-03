using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class UserSettings
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("locale")]
	public string Locale { get; set; }

	[JsonProperty("theme")]
	public string Theme { get; set; }

	[JsonProperty("status")]
	public string Status { get; set; }

	[JsonProperty("custom_status")]
	public UserCustomStatus? CustomStatus { get; set; }

	[JsonProperty("developer_mode")]
	public bool DeveloperMode { get; set; }

	[JsonProperty("message_display_compact")]
	public bool CompactMessageDisplay { get; set; }

	[JsonProperty("animate_emoji")]
	public bool AnimateEmoji { get; set; }

	[JsonProperty("animate_stickers")]
	public int AnimateStickers { get; set; }

	[JsonProperty("gif_auto_play")]
	public bool GifAutoPlay { get; set; }

	[JsonProperty("render_embeds")]
	public bool RenderEmbeds { get; set; }

	[JsonProperty("render_reactions")]
	public bool RenderReactions { get; set; }

	[JsonProperty("render_spoilers")]
	public int RenderSpoilers { get; set; }

	[JsonProperty("inline_attachment_media")]
	public bool InlineAttachmentMedia { get; set; }

	[JsonProperty("inline_embed_media")]
	public bool InlineEmbedMedia { get; set; }

	[JsonProperty("explicit_content_filter")]
	public int ExplicitContentFilter { get; set; }

	[JsonProperty("friend_source_flags")]
	public int FriendSourceFlags { get; set; }

	[JsonProperty("incoming_call_flags")]
	public int IncomingCallFlags { get; set; }

	[JsonProperty("group_dm_add_permission_flags")]
	public int GroupDmAddPermissionFlags { get; set; }

	[JsonProperty("default_guilds_restricted")]
	public bool DefaultGuildsRestricted { get; set; }

	[JsonProperty("restricted_guilds")]
	public List<ulong>? RestrictedGuilds { get; set; }

	[JsonProperty("guild_positions")]
	public List<ulong>? GuildPositions { get; set; }

	[JsonProperty("guild_folders")]
	public List<UserGuildFolder>? GuildFolders { get; set; }

	[JsonProperty("afk_timeout")]
	public int AfkTimeout { get; set; }

	[JsonProperty("time_format")]
	public int TimeFormat { get; set; }
}

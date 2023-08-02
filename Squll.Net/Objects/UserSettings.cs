#nullable enable
#pragma warning disable CS8618

using Newtonsoft.Json;
using Squll.Net.Objects.DataTables;

namespace Squll.Net.Objects;

public class UserSettings
{
    [JsonProperty("animate_expressions")]
    public bool AnimateExpressions { get; set; }
    [JsonProperty("convert_emoticons")]
    public bool ConvertEmoticons { get; set; }
    [JsonProperty("custom_status")]
    public string? CustomStatus { get; set; }
    [JsonProperty("default_squads_restricted")]
    public bool DefaultSquadsRestricted { get; set; }
    [JsonProperty("developer_mode")]
    public bool DeveloperMode { get; set; }
    [JsonProperty("friend_source_flags")]
    public FriendSourceFlags FriendSourceFlags { get; set; }
    [JsonProperty("gif_auto_play")]
    public bool GifAutoPlay { get; set; }
    [JsonProperty("idle_timeout")]
    public int IdleTimeout { get; set; }
    [JsonProperty("inline_attachment_media")]
    public bool InlineAttachmentMedia { get; set; }
    [JsonProperty("inline_embed_media")]
    public bool InlineEmbedMedia { get; set; }
    [JsonProperty("locale")]
    public string Locale { get; set; }
    [JsonProperty("message_display_compact")]
    public bool MessageDisplayCompact { get; set; }
    [JsonProperty("nsfw_filter_level")]
    public NSFWFilterLevelType NSFWFilterLevel { get; set; }
    [JsonProperty("render_embeds")]
    public bool RenderEmbeds { get; set; }
    [JsonProperty("render_reactions")]
    public bool RenderReactions { get; set; }
    [JsonProperty("restricted_squads")]
    public object[]? RestrictedSquads { get; set; }
    [JsonProperty("squad_folders")]
    public object[]? SquadFolders { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("theme")]
    public string Theme { get; set; }
}

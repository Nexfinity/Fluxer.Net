#nullable enable
#pragma warning disable CS8618

using Newtonsoft.Json;
using Fluxer.Net.Objects.Data;

namespace Fluxer.Net.Objects;

public class UserSettings
{
    [JsonProperty("animate_expressions")]
    public bool AnimateExpressions { get; set; }
    [JsonProperty("convert_emoticons")]
    public bool ConvertEmoticons { get; set; }
    [JsonProperty("custom_status")]
    public string? CustomStatus { get; set; }
    [JsonProperty("default_communitys_restricted")]
    public bool DefaultCommunitysRestricted { get; set; }
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
    [JsonProperty("restricted_communitys")]
    public object[]? RestrictedCommunitys { get; set; }
    [JsonProperty("community_folders")]
    public object[]? CommunityFolders { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("theme")]
    public string Theme { get; set; }
}

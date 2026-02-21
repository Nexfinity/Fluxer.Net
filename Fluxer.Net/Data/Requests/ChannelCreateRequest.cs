using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ChannelCreateTextRequest), "GUILD_TEXT")]
[JsonDerivedType(typeof(ChannelCreateVoiceRequest), "GUILD_VOICE")]
[JsonDerivedType(typeof(ChannelCreateCategoryRequest), "GUILD_CATEGORY")]
[JsonDerivedType(typeof(ChannelCreateLinkRequest), "GUILD_LINK")]
public class ChannelCreateRequest
{
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonPropertyName("parent_id")]
    public ulong? ParentCategoryId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [DefaultValue(false)]
    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }
    
    [JsonPropertyName("permission_overwrites")]
    public ChannelOverwriteRequest[]? PermissionOverwrites { get; set; }
}

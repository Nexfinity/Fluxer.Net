using Newtonsoft.Json;
using Fluxer.Net.Objects.Data;

namespace Fluxer.Net.Objects;

public class Role
{
    [JsonProperty("color")]
    public uint Color { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("flags")]
    public RoleFlags Flags { get; set; }
    [JsonProperty("icon")]
    public string Icon { get; set; }
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("mention_privacy_level")]
    public MentionPrivacyLevel MentionPrivacyLevel { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("permissions")]
    public Permissions Permissions { get; set; }
    [JsonProperty("position")]
    public int Position { get; set; }
    [JsonProperty("community_id")]
    public ulong CommunityId { get; set; }
    [JsonProperty("unicode_emoji")]
    public string UnicodeEmoji { get; set; }
}

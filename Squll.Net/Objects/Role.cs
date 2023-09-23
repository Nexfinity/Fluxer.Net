using Newtonsoft.Json;
using Squll.Net.Objects.Data;

namespace Squll.Net.Objects;

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
    public Permissions[] Permissions { get; set; }
    [JsonProperty("position")]
    public int Position { get; set; }
    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }
    [JsonProperty("unicode_emoji")]
    public string UnicodeEmoji { get; set; }
}

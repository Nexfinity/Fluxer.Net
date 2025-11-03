using Fluxer.Net.Data.Enums;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class Role
{
    [JsonProperty("color")]
    public uint Color { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("flags")]
    public int Flags { get; set; }
    [JsonProperty("icon")]
    public string Icon { get; set; }
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("mention_privacy_level")]
    public int MentionPrivacyLevel { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("permissions")]
    public Permissions Permissions { get; set; }
    [JsonProperty("position")]
    public int Position { get; set; }
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }
    [JsonProperty("unicode_emoji")]
    public string UnicodeEmoji { get; set; }
}

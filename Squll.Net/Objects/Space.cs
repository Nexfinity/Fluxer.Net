using Newtonsoft.Json;
using Squll.Net.Objects.DataTables;

namespace Squll.Net.Objects;

public class Space
{
    [JsonProperty("color")]
    public uint Color { get; set; }
    [JsonProperty("cooldown")]
    public int Cooldown { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("flags")]
    public SpaceFlags Flags { get; set; }
    [JsonProperty("icon")]
    public string? Icon { get; set; }
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("last_message_id")]
    public ulong? LastMessageId { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("overwrites")]
    public object[] Overwrites { get; set; }
    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }
    [JsonProperty("position")]
    public int Position { get; set; }
    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }
    [JsonProperty("type")]
    public SpaceType Type { get; set; }
    [JsonProperty("unicode_emoji")]
    public string UnicodeEmoji { get; set; }
}

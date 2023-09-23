using Newtonsoft.Json;
using Squll.Net.Objects.Data;

namespace Squll.Net.Objects;

public class Channel
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

    [JsonProperty("unicode_emoji")]
    public string UnicodeEmoji { get; set; }

    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    [JsonProperty("last_message_id")]
    public ulong? LastMessageId { get; set; }

    [JsonProperty("type")]
    public ChannelType Type { get; set; }

    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("color")]
    public uint Color { get; set; }

    [JsonProperty("cooldown")]
    public int Cooldown { get; set; }

    [JsonProperty("flags")]
    public ChannelFlags Flags { get; set; }

    [JsonProperty("overwrites")]
    public object[] Overwrites { get; set; }
}

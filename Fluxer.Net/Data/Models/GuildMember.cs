using System;
using Newtonsoft.Json;
using Fluxer.Net.Objects.Data;

namespace Fluxer.Net.Objects.Models;

public class GuildMember
{
    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    [JsonProperty("avatar_decoration")]
    public string? AvatarDecoration { get; set; }

    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    [JsonProperty("roles")]
    public List<ulong> Roles { get; set; }

    [JsonProperty("flags")]
    public int Flags { get; set; }

    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }
}

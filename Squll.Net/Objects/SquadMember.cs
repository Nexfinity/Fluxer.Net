using System;
using Newtonsoft.Json;
using Squll.Net.Objects.DataTables;

namespace Squll.Net.Objects;

public class SquadMember
{
    [JsonProperty("flags")]
    public SquadMemberFlags Flags { get; set; }
    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }
    [JsonProperty("mention_privacy_level")]
    public MentionPrivacyLevel MentionPrivacyLevel { get; set; }
    [JsonProperty("display_name ")]
    public string? DisplayName { get; set; }
    [JsonProperty("roles")]
    public List<ulong> Roles { get; set; }
    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }
    [JsonProperty("user")]
    public User User { get; set; }
    [JsonProperty("avatar")]
    public string? Avatar { get; set; }
    [JsonProperty("avatar_decoration")]
    public string? AvatarDecoration { get; set; }
    [JsonProperty("invite_code")]
    public string? InviteCode { get; set; }
}

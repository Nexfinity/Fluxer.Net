using System;
using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class SquadMember
{
    [JsonProperty("flags")]
    public SquadMemberFlags Flags { get; set; }
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }
    [JsonProperty("mention_privacy_level")]
    public MentionPrivacyLevel MentionPrivacyLevel { get; set; }
    [JsonProperty("nickname")]
    public string? Nickname { get; set; }
    [JsonProperty("roles")]
    public Role[] Roles { get; set; }
    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }
    [JsonProperty("user")]
    public User User { get; set; }
}

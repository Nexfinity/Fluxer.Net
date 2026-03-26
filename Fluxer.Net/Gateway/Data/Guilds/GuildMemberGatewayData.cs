using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Guilds;

/// <summary>
/// Gateway guild member data matching the GuildMemberResponse API model
/// </summary>
public class GuildMemberGatewayData
{
    [JsonProperty("user")]
    public UserJson User { get; set; } = null!;

    [JsonProperty("nick")]
    public string? Nick { get; set; }

    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    [JsonProperty("banner")]
    public string? Banner { get; set; }

    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    [JsonProperty("roles")]
    public List<ulong> Roles { get; set; } = new();

    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }

    [JsonProperty("join_source_type")]
    public int? JoinSourceType { get; set; }

    [JsonProperty("source_invite_code")]
    public string? SourceInviteCode { get; set; }

    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }

    [JsonProperty("mute")]
    public bool IsMuted { get; set; }

    [JsonProperty("deaf")]
    public bool IsDeafened { get; set; }

    /// <summary>
    /// Guild ID for context (added for gateway events)
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}

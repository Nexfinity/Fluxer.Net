using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildGatewayData
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("joined_at")]
    public DateTimeOffset? JoinedAt { get; set; }

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("online_count")]
    public int OnlineCount { get; set; }

    [JsonProperty("unavailable")]
    public bool? IsUnavailable { get; set; }

    [JsonProperty("properties")]
    public GuildJson? Properties { get; set; }

    [JsonProperty("channels")]
    public List<ChannelGatewayData> Channels { get; set; } = new();

    [JsonProperty("members")]
    public List<GuildMemberGatewayData> Members { get; set; }

    [JsonProperty("roles")]
    public List<RoleJson> Roles { get; set; }

    [JsonProperty("emojis")]
    public GuildEmojiJson[]? Emojis { get; set; }

    [JsonProperty("stickers")]
    public GuildStickerJson[]? Stickers { get; set; }

    [JsonProperty("voice_states")]
    public List<VoiceStateJson>? VoiceStates { get; set; }
}
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class ReadyGatewayData
{
    [JsonProperty("v")]
    public string Version { get; set; }

    [JsonProperty("auth_session_id_hash")]
    public string AuthSessionIdHash { get; set; }

    [JsonProperty("country_code")]
    public string CountryCode { get; set; }

    [JsonProperty("favorite_memes")]
    public FavoriteGifJson[]? FavoriteMemes { get; set; }

    [JsonProperty("pinned_dms")]
    public ulong[] PinnedDMs { get; set; }

    [JsonProperty("notes")]
    public Dictionary<string, string> Notes { get; set; }

    [JsonProperty("private_channels")]
    public ChannelJson[]? PrivateChannels { get; set; }

    [JsonProperty("relationships")]
    public RelationshipJson[] Relationships { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    [JsonProperty("sessions")]
    public GatewaySessionJson[]? Sessions { get; set; }

    [JsonProperty("guilds")]
    public GuildGatewayData[] Guilds { get; set; }

    [JsonProperty("user")]
    public CurrentUserJson User { get; set; }

    [JsonProperty("user_settings")]
    public UserSettingsJson? UserSettings { get; set; }

    [JsonProperty("user_guild_settings")]
    public UserGuildSettingsJson[]? UserGuildSettings { get; set; }

    [JsonProperty("rtc_regions")]
    public RtcRegionJson[]? RtcRegions { get; set; }
}

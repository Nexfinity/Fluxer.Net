using Newtonsoft.Json;

namespace Fluxer.Net.Objects;

public class GuildMemberProfile : UserProfile
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }
}

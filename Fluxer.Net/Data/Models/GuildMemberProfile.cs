using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class GuildMemberProfile : UserProfile
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }
}

using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class SquadMemberProfile : UserProfile
{
    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }
}

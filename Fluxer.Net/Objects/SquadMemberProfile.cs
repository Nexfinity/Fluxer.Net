using Newtonsoft.Json;

namespace Fluxer.Net.Objects;

public class CommunityMemberProfile : UserProfile
{
    [JsonProperty("community_id")]
    public ulong CommunityId { get; set; }
}

using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Guilds;

public class GuildMembersChunkGatewayData
{
    [JsonProperty("chunk_count")]
    public int ChunkCount { get; set; }

    [JsonProperty("chunk_index")]
    public int ChunkIndex { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("members")]
    public GuildMemberJson[] Members { get; set; }
}

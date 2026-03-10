using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

public class GlobalSearchMessagesRequest : MessageSearchRequest
{
    /// <summary>
    /// Channel ID for context when searching across multiple channels
    /// </summary>
    [JsonProperty("context_channel_id")]
    public ulong? ContextChannelId { get; set; }

    /// <summary>
    /// Guild ID for context when searching across multiple guilds
    /// </summary>
    [JsonProperty("context_build_ids")]
    public ulong? ContextGuildId { get; set; }

    /// <summary>
    /// Specific channel IDs to search in
    /// </summary>
    [MaxLength(500)]
    [JsonProperty("channel_ids")]
    public HashSet<ulong>? SpecificChannelIds { get; set; }
}

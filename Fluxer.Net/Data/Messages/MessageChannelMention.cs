using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A channel mentioned in a message that everyone in the guild can see.
/// </summary>
public class MessageChannelMention
{
    /// <summary>
    /// The ID of the mentioned channel.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The name of the mentioned channel.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of the mentioned channel.
    /// </summary>
    [JsonProperty("type")]
    public ChannelType Type { get; set; }
}

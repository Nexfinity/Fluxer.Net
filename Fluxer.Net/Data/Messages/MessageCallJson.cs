using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class MessageCallJson : IMessageCall
{
    /// <inheritdoc />
    [JsonProperty("participants")]
    public HashSet<ulong> Participants { get; set; }

    /// <inheritdoc />
    [JsonProperty("ended_timestamp")]
    public DateTimeOffset? EndedAt { get; set; }
}

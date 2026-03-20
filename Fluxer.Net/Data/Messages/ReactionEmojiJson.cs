using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L71"/>
/// </remarks>
public class ReactionEmojiJson
{
    [JsonProperty("id")]
    public ulong? Id { get; set; }

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("animated")]
    public bool? Animated { get; set; }
}

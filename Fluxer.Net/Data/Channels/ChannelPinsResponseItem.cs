using Newtonsoft.Json;

namespace Fluxer.Net.Data.Channels;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L185"/>
/// </remarks>
public class ChannelPinsResponseItem
{
    [JsonRequired]
    [JsonProperty("message")]
    public ChannelPinMessageResponse Message { get; set; }

    /// <summary>
    /// The ISO 8601 timestamp of when the message was pinned
    /// </summary>
    [JsonRequired]
    [JsonProperty("pinned_at")]
    public DateTime PinnedAt { get; set; }
}

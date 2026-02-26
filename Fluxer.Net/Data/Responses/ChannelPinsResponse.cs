using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L192"/>
/// </remarks>
public class ChannelPinsResponse
{
    /// <summary>
    /// Pinned messages in this channel
    /// </summary>
    [JsonRequired]
    [JsonProperty("items")]
    public ChannelPinsResponseItem[] Items { get; set; } = Array.Empty<ChannelPinsResponseItem>();

    /// <summary>
    /// Whether more pins can be fetched with pagination
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }
}

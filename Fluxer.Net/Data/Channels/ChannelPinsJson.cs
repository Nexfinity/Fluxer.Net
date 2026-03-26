using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L192"/>
/// </remarks>
public class ChannelPinsJson : IChannelPins
{
    /// <inheritdoc />
    [JsonRequired]
    [JsonProperty("items")]
    public ChannelPinJson[] Items { get; set; }

    /// <inheritdoc />
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    IEnumerable<IChannelPin> IChannelPins.Items => Items;
}

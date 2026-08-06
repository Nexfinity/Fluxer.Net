using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
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

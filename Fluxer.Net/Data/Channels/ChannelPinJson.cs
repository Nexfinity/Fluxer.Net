using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L185"/>
/// </remarks>
public class ChannelPinJson : IChannelPin
{
    /// <inheritdoc />
    [JsonProperty("message")]
    public MessageJson Message { get; set; }

    /// <inheritdoc />
    [JsonProperty("pinned_at")]
    public DateTime PinnedAt { get; set; }

    IMessage IChannelPin.Message => Message;
}

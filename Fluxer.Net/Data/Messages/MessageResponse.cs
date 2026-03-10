using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L173"/>
/// </remarks>
public class MessageResponse : MessageBaseResponse
{
    /// <summary>
    /// The message that this message is replying to or forwarding
    /// </summary>
    [JsonProperty("referenced_message")]
    public MessageResponse? ReferencedMessage { get; set; }
}

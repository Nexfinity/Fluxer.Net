using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A voice state update Fluxer refused, sent only to the session that sent it.
/// </summary>
public class GatewayErrorGatewayData
{
    /// <summary>
    /// The refusal code, one of the voice codes in <see cref="FluxerErrorCodes"/>.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; }

    /// <summary>
    /// A description of the refusal, meant for logs.
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; }
}

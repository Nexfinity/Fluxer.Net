using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/channel/ChannelSchemas.tsx#L27"/>
/// </remarks>
public class PermissionOverwriteJson : IPermissionOverwrite
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public int Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("allow")]
    public ulong Allow { get; set; }

    /// <inheritdoc />
    [JsonProperty("deny")]
    public ulong Deny { get; set; }
}

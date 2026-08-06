using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
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
    public ChannelPermissions Allow { get; set; }

    /// <inheritdoc />
    [JsonProperty("deny")]
    public ChannelPermissions Deny { get; set; }
}

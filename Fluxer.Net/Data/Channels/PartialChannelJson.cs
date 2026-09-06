using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class PartialChannelJson : IPartialChannel
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    [JsonIgnore]
    public string Mention => $"<#{Id}>";

    /// <inheritdoc />
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public ChannelType Type { get; set; }

    /// <inheritdoc/>
    public bool IsTextable => Channel.TextableTypes(Type);
}

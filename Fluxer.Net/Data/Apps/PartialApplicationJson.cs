using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class PartialApplicationJson : IPartialApplication
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon")]
    public string Icon { get; set; }

    /// <inheritdoc />
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <inheritdoc />
    [JsonProperty("bot_public")]
    public bool IsPublic { get; set; }

    /// <inheritdoc />
    [JsonProperty("bot_requires_code_grant")]
    public bool RequiresCodeGrant { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public ulong Flags { get; set; }
}

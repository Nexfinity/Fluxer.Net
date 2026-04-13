using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class UserGuildFolderJson : IUserGuildFolder
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_ids")]
    public List<ulong>? GuildIds { get; set; }
}

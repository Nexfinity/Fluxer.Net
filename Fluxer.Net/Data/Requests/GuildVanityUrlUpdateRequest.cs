using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

/// <remarks>
/// <see href="https://docs.fluxer.app/resources/guilds#guildvanityurlupdaterequest"/>
/// </remarks>
public class GuildVanityUrlUpdateRequest
{
    /// <summary>
    /// The new vanity Url code (2-32 characters, alphanumeric and hyphens)
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

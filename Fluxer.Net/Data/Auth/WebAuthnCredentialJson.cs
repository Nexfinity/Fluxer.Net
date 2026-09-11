using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class WebAuthnCredentialJson : IWebAuthnCredential
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("last_used_at")]
    public DateTimeOffset? LastUsedAt { get; set; }
}

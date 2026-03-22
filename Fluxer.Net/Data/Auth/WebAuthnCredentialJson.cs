using Newtonsoft.Json;

namespace Fluxer.Net;

public class WebAuthnCredentialJson
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("credential_id")]
    public string CredentialId { get; set; }

    [JsonProperty("public_key")]
    public byte[] PublicKey { get; set; }

    [JsonProperty("counter")]
    public ulong Counter { get; set; }

    [JsonProperty("transports")]
    public HashSet<string>? Transports { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("last_used_at")]
    public DateTime? LastUsedAt { get; set; }
}

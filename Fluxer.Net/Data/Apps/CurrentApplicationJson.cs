using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentApplicationJson : ApplicationJson, IApplication, ICurrentApplication
{
    /// <inheritdoc />
    [JsonProperty("owner")]
    public UserJson Owner { get; set; }

    /// <summary>
    /// The public key used to verify requests signed by Fluxer for this application.
    /// </summary>
    [JsonProperty("verify_key")]
    public string? VerifyKey { get; set; }

    IUser ICurrentApplication.Owner => Owner;
}
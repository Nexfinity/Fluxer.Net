using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentApplicationJson : ApplicationJson, IApplication, ICurrentApplication
{
    /// <inheritdoc />
    [JsonProperty("owner")]
    public UserJson Owner { get; set; }

    IUser ICurrentApplication.Owner => Owner;
}
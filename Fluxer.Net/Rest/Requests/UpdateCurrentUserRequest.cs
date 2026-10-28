using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating the current user's profile, sending only the properties that are set.
/// </summary>
public class UpdateCurrentUserRequest
{
    [JsonProperty("username")]
    public Optional<string> Username { get; set; }

    [JsonProperty("global_name")]
    public Optional<string?> DisplayName { get; set; }

    [JsonProperty("avatar")]
    public Optional<string?> AvatarBase64 { get; set; }

    [JsonProperty("banner")]
    public Optional<string?> BannerBase64 { get; set; }

    [JsonProperty("bio")]
    public Optional<string?> Bio { get; set; }

    public void AvatarFromStream(Stream stream) => AvatarBase64 = ToBase64(stream);

    public void BannerFromStream(Stream stream) => BannerBase64 = ToBase64(stream);

    private static string ToBase64(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return Convert.ToBase64String(ms.ToArray());
    }
}

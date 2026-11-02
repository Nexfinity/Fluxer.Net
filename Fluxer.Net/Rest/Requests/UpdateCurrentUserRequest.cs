using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating the current user's profile, sending only the properties that are set.
/// </summary>
public class UpdateCurrentUserRequest
{
    /// <summary>
    /// The new username of the current user.
    /// </summary>
    [JsonProperty("username")]
    public Optional<string> Username { get; set; }

    /// <summary>
    /// The new display name of the current user.
    /// </summary>
    [JsonProperty("global_name")]
    public Optional<string?> DisplayName { get; set; }

    /// <summary>
    /// The new avatar as base64-encoded image data.
    /// </summary>
    [JsonProperty("avatar")]
    public Optional<string?> AvatarBase64 { get; set; }

    /// <summary>
    /// The new banner as base64-encoded image data.
    /// </summary>
    [JsonProperty("banner")]
    public Optional<string?> BannerBase64 { get; set; }

    /// <summary>
    /// The new bio of the current user.
    /// </summary>
    [JsonProperty("bio")]
    public Optional<string?> Bio { get; set; }

    /// <summary>
    /// Sets the avatar from the image data read from a stream.
    /// </summary>
    /// <param name="stream">The stream holding the image data.</param>
    public void AvatarFromStream(Stream stream) => AvatarBase64 = ToBase64(stream);

    /// <summary>
    /// Sets the banner from the image data read from a stream.
    /// </summary>
    /// <param name="stream">The stream holding the image data.</param>
    public void BannerFromStream(Stream stream) => BannerBase64 = ToBase64(stream);

    private static string ToBase64(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return Convert.ToBase64String(ms.ToArray());
    }
}

using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating the current user's member profile in a guild, sending only the properties that are set.
/// </summary>
public class UpdateCurrentMemberRequest
{
    /// <summary>
    /// The nickname, 1 to 32 characters, or null to clear it.
    /// </summary>
    [JsonProperty("nick")]
    public Optional<string?> Nickname { get; set; }

    /// <summary>
    /// Base64 image data for the guild avatar, or null to remove it.
    /// </summary>
    [JsonProperty("avatar")]
    public Optional<string?> AvatarBase64 { get; set; }

    /// <summary>
    /// Base64 image data for the guild banner, or null to remove it.
    /// </summary>
    [JsonProperty("banner")]
    public Optional<string?> BannerBase64 { get; set; }

    /// <summary>
    /// The guild profile bio, 1 to 320 characters, or null to clear it.
    /// </summary>
    [JsonProperty("bio")]
    public Optional<string?> Bio { get; set; }

    /// <summary>
    /// The guild profile pronouns, 1 to 40 characters, or null to clear them.
    /// </summary>
    [JsonProperty("pronouns")]
    public Optional<string?> Pronouns { get; set; }

    /// <summary>
    /// The guild profile accent color as an RGB integer, or null to clear it.
    /// </summary>
    [JsonProperty("accent_color")]
    public Optional<int?> AccentColor { get; set; }

    /// <summary>
    /// Flags on the guild profile.
    /// </summary>
    [JsonProperty("profile_flags")]
    public Optional<GuildMemberProfileFlags?> ProfileFlags { get; set; }

    /// <summary>
    /// Whether replies to this member mention them by default in this guild.
    /// </summary>
    [JsonProperty("mention_flags")]
    public Optional<MentionReplyPreference?> MentionFlags { get; set; }

    /// <summary>
    /// Whether the member is muted in voice channels.
    /// </summary>
    [JsonProperty("mute")]
    public Optional<bool> IsMute { get; set; }

    /// <summary>
    /// Whether the member is deafened in voice channels.
    /// </summary>
    [JsonProperty("deaf")]
    public Optional<bool> IsDeaf { get; set; }

    /// <summary>
    /// The voice channel to move to, or null to disconnect.
    /// </summary>
    [JsonProperty("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    /// <summary>
    /// The voice connection to move when <see cref="ChannelId"/> is set.
    /// </summary>
    [JsonProperty("connection_id")]
    public Optional<string?> ConnectionId { get; set; }

    /// <summary>
    /// Sets <see cref="AvatarBase64"/> from an image stream.
    /// </summary>
    public void AvatarFromStream(Stream stream) => AvatarBase64 = ToBase64(stream);

    /// <summary>
    /// Sets <see cref="BannerBase64"/> from an image stream.
    /// </summary>
    public void BannerFromStream(Stream stream) => BannerBase64 = ToBase64(stream);

    private static string ToBase64(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return Convert.ToBase64String(ms.ToArray());
    }
}

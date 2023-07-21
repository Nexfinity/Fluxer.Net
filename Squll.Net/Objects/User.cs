using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class User
{
    [JsonProperty("avatar")]
    public string Avatar { get; set; }
    [JsonProperty("avatar_decoration")]
    public object? AvatarDecoration { get; set; }
    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }
    [JsonProperty("display_name")]
    public string DisplayName { get; set; }
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("public_flags")]
    public UserPublicFlags PublicFlags { get; set; }
    [JsonProperty("timezone_offset")]
    public int? TimezoneOffsett { get; set; }
    [JsonProperty("type")]
    public UserType Type { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("accent_color")]
    public object? AccentColor { get; set; }
    [JsonProperty("banner")]
    public string? Banner { get; set; }
    [JsonProperty("biography")]
    public string? Biography { get; set; }
    [JsonProperty("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }
    [JsonProperty("email")]
    public string? Email { get; set; }
    [JsonProperty("location")]
    public string? Location { get; set; }
    [JsonProperty("mention_privacy_level")]
    public MentionPrivacyLevel? MentionPrivacyLevel { get; set; }
    [JsonProperty("mfa_flags")]
    public MfaFlags? MfaFlags { get; set; }
    [JsonProperty("premium_since")]
    public DateTime? PremiumSince { get; set; }
    [JsonProperty("premium_type")]
    public UserPremiumType? PremiumType { get; set; }
    [JsonProperty("premium_usage_flags")]
    public PremiumUsageFlags? PremiumUsageFlags { get; set; }
    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }
}

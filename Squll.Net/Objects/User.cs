using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Squll.Net.Objects.Data;

namespace Squll.Net.Objects;

public class User
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("discriminator")]
    public string? Discriminator { get; set; }

    [JsonProperty("type")]
    public UserType Type { get; set; }

    [JsonProperty("display_name")]
    public string DisplayName { get; set; }

    [JsonProperty("avatar")]
    public string Avatar { get; set; }

    [JsonProperty("avatar_decoration")]
    public object? AvatarDecoration { get; set; }

    [JsonProperty("flags")]
    public UserFlags Flags { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("timezone")]
    public string? Timezone { get; set; }

    [JsonProperty("date_of_Birth")]
    public DateOnly? DateOfBirth { get; set; }

    [JsonProperty("birthday_visibility")]
    public BirthdayVisibility BirthdayVisibility { get; set; }

    [JsonProperty("premium_usage_flags")]
    public UserPremiumUsageFlags? PremiumUsageFlags { get; set; }

    [JsonProperty("premium_type")]
    public PremiumType? PremiumType { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }

    [JsonProperty("new_password")]
    public string? NewPassword { get; set; }


}


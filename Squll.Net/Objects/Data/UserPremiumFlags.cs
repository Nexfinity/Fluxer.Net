namespace Squll.Net.Objects.Data;

[Flags]
public enum UserPremiumUsageFlags
{
    None = 0,
    PremiumDiscriminator = 1 << 0,
    AnimatedAvatar = 1 << 1,
    AnimatedAvatarDecoration = 1 << 2,
    AnimatedBanner = 1 << 3,
}

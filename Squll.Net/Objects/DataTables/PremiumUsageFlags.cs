namespace Squll.Net.Objects;

public enum PremiumUsageFlags
{
    None = 0,
    PremiumDiscriminator = 1 << 0,
    AnimatedAvatar = 1 << 1,
    ProfileBanner = 1 << 2,
}

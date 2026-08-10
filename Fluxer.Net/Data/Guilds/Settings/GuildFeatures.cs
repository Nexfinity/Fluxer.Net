namespace Fluxer.Net;

public class GuildFeatures
{
    private GuildFeatures(string[]? features)
    {
        Raw = features ??= [];
        if (features != null)
        {
            foreach (string feature in features)
            {
                switch (feature)
                {
                    case "ANIMATED_ICON":
                        HasAnimatedIcon = true;
                        break;
                    case "ANIMATED_BANNER":
                        HasAnimatedBanner = true;
                        break;
                    case "BANNER":
                        HasBanner = true;
                        break;
                    case "DETACHED_BANNER":
                        IsBannerDetached = true;
                        break;
                    case "INVITE_SPLASH":
                        HasInviteSplash = true;
                        break;
                    case "INVITES_DISABLED":
                        IsInvitesDisabled = true;
                        break;
                    case "TEXT_CHANNEL_FLEXIBLE_NAMES":
                        IsChannelNamesFlexible = true;
                        break;
                    case "MORE_EMOJI":
                        HasMoreEmojis = true;
                        break;
                    case "MORE_STICKERS":
                        HasMoreStickers = true;
                        break;
                    case "UNLIMITED_EMOJI":
                        HasUnlimitedEmojis = true;
                        break;
                    case "UNLIMITED_STICKERS":
                        HasUnlimitedStickers = true;
                        break;
                    case "EXPRESSION_PURGE_ALLOWED":
                        IsExpressionPurgeAllowed = true;
                        break;
                    case "VANITY_URL":
                        HasVanityUrl = true;
                        break;
                    case "DISCOVERABLE":
                        IsDiscoverable = true;
                        break;
                    case "PARTNERED":
                        IsPartnered = true;
                        break;
                    case "VERIFIED":
                        IsVerified = true;
                        break;
                    case "VIP_VOICE":
                        HasVipVoice = true;
                        break;
                    case "UNAVAILABLE_FOR_EVERYONE":
                        IsUnavailable = true;
                        break;
                    case "UNAVAILABLE_FOR_EVERYONE_BUT_STAFF":
                        IsStaffOnly = true;
                        break;
                    case "VISIONARY":
                        IsVisionary = true;
                        break;
                    case "OPERATOR":
                        IsOperator = true;
                        break;
                    case "DISALLOW_UNCLAIMED_ACCOUNTS":
                        BlockUnclaimedAccounts = true;
                        break;
                    case "LARGE_GUILD_OVERRIDE":
                        HasLargeGuildOverride = true;
                        break;
                    case "VERY_LARGE_GUILD":
                        IsLargeServer = true;
                        break;
                }
            }
        }
    }

    public string[] Raw { get; }
    public bool HasAnimatedIcon { get; }
    public bool HasAnimatedBanner { get; }
    public bool HasBanner { get; }
    public bool IsBannerDetached { get; }
    public bool HasInviteSplash { get; }
    public bool IsInvitesDisabled { get; }
    public bool IsChannelNamesFlexible { get; }
    public bool HasMoreEmojis { get; }
    public bool HasMoreStickers { get; }
    public bool HasUnlimitedEmojis { get; }
    public bool HasUnlimitedStickers { get; }
    public bool IsExpressionPurgeAllowed { get; }
    public bool HasVanityUrl { get; }
    public bool IsDiscoverable { get; }
    public bool IsPartnered { get; }
    public bool IsVerified { get; }
    public bool HasVipVoice { get; }
    public bool IsUnavailable { get; }
    public bool IsStaffOnly { get; }
    public bool IsVisionary { get; }
    public bool IsOperator { get; }
    public bool BlockUnclaimedAccounts { get; }
    public bool HasLargeGuildOverride { get; }
    public bool IsLargeServer { get; }

    public static GuildFeatures FromServer(PartialGuildJson guild)
    {
        return new GuildFeatures(guild.Features);
    }
}

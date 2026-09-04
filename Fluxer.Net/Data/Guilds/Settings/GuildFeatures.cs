namespace Fluxer.Net;

public class GuildFeatures
{
    public string[] Raw { get; private set; }
    public bool HasAnimatedIcon { get; private set; }
    public bool HasAnimatedBanner { get; private set; }
    public bool HasBanner { get; private set; }
    public bool IsBannerDetached { get; private set; }
    public bool HasInviteSplash { get; private set; }
    public bool IsInvitesDisabled { get; private set; }
    public bool IsChannelNamesFlexible { get; private set; }
    public bool HasMoreEmojis { get; private set; }
    public bool HasMoreStickers { get; private set; }
    public bool HasUnlimitedEmojis { get; private set; }
    public bool HasUnlimitedStickers { get; private set; }
    public bool IsExpressionPurgeAllowed { get; private set; }
    public bool HasVanityUrl { get; private set; }
    public bool IsDiscoverable { get; private set; }
    public bool IsPartnered { get; private set; }
    public bool IsVerified { get; private set; }
    public bool HasVipVoice { get; private set; }
    public bool IsUnavailable { get; private set; }
    public bool IsStaffOnly { get; private set; }
    public bool IsVisionary { get; private set; }
    public bool IsOperator { get; private set; }
    public bool BlockUnclaimedAccounts { get; private set; }
    public bool HasLargeGuildOverride { get; private set; }
    public bool IsLargeServer { get; private set; }

    internal GuildFeatures()
    {

    }

    public static GuildFeatures Create(string[]? features)
    {
        GuildFeatures data = new GuildFeatures
        {
            Raw = features ??= []
        };

        if (features != null)
        {
            foreach (string feature in features)
            {
                switch (feature)
                {
                    case "ANIMATED_ICON":
                        data.HasAnimatedIcon = true;
                        break;
                    case "ANIMATED_BANNER":
                        data.HasAnimatedBanner = true;
                        break;
                    case "BANNER":
                        data.HasBanner = true;
                        break;
                    case "DETACHED_BANNER":
                        data.IsBannerDetached = true;
                        break;
                    case "INVITE_SPLASH":
                        data.HasInviteSplash = true;
                        break;
                    case "INVITES_DISABLED":
                        data.IsInvitesDisabled = true;
                        break;
                    case "TEXT_CHANNEL_FLEXIBLE_NAMES":
                        data.IsChannelNamesFlexible = true;
                        break;
                    case "MORE_EMOJI":
                        data.HasMoreEmojis = true;
                        break;
                    case "MORE_STICKERS":
                        data.HasMoreStickers = true;
                        break;
                    case "UNLIMITED_EMOJI":
                        data.HasUnlimitedEmojis = true;
                        break;
                    case "UNLIMITED_STICKERS":
                        data.HasUnlimitedStickers = true;
                        break;
                    case "EXPRESSION_PURGE_ALLOWED":
                        data.IsExpressionPurgeAllowed = true;
                        break;
                    case "VANITY_URL":
                        data.HasVanityUrl = true;
                        break;
                    case "DISCOVERABLE":
                        data.IsDiscoverable = true;
                        break;
                    case "PARTNERED":
                        data.IsPartnered = true;
                        break;
                    case "VERIFIED":
                        data.IsVerified = true;
                        break;
                    case "VIP_VOICE":
                        data.HasVipVoice = true;
                        break;
                    case "UNAVAILABLE_FOR_EVERYONE":
                        data.IsUnavailable = true;
                        break;
                    case "UNAVAILABLE_FOR_EVERYONE_BUT_STAFF":
                        data.IsStaffOnly = true;
                        break;
                    case "VISIONARY":
                        data.IsVisionary = true;
                        break;
                    case "OPERATOR":
                        data.IsOperator = true;
                        break;
                    case "DISALLOW_UNCLAIMED_ACCOUNTS":
                        data.BlockUnclaimedAccounts = true;
                        break;
                    case "LARGE_GUILD_OVERRIDE":
                        data.HasLargeGuildOverride = true;
                        break;
                    case "VERY_LARGE_GUILD":
                        data.IsLargeServer = true;
                        break;
                }
            }
        }

        return data;
    }

    public static GuildFeatures FromServer(PartialGuildJson guild)
    {
        return GuildFeatures.Create(guild.Features);
    }
}

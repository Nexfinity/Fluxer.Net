namespace Fluxer.Net;

public class GuildFeatures
{
    public string[] Raw { get; private set; }
    public bool AnimatedIcon { get; private set; }
    public bool AnimatedBanner { get; private set; }
    public bool CloneEmoji { get; private set; }
    public bool CloneSticker { get; private set; }
    public bool AudioBitrate128Kbps { get; private set; }
    public bool AudioBitrate256Kbps { get; private set; }
    public bool AudioBitrate384Kbps { get; private set; }
    public bool Banner { get; private set; }
    public bool IsBannerDetached { get; private set; }
    public bool InviteSplash { get; private set; }
    public bool IsInvitesDisabled { get; private set; }
    public bool RaidDetected { get; private set; }
    public bool IsChannelNamesFlexible { get; private set; }
    public bool HideOwnerCrown { get; private set; }
    public bool MoreEmojis { get; private set; }
    public bool MoreStickers { get; private set; }
    public bool UnlimitedEmojis { get; private set; }
    public bool UnlimitedStickers { get; private set; }
    public bool IsExpressionPurgeAllowed { get; private set; }
    public bool VanityUrl { get; private set; }
    public bool IsDiscoverable { get; private set; }
    public bool IsPartnered { get; private set; }
    public bool IsVerified { get; private set; }
    public bool VoiceE2EE { get; private set; }
    public bool VipVoice { get; private set; }
    public bool IsUnavailable { get; private set; }
    public bool IsHidden { get; private set; }
    public bool IsStaffOnly { get; private set; }
    public bool IsVisionary { get; private set; }
    public bool IsOperator { get; private set; }
    public bool BlockUnclaimedAccounts { get; private set; }
    public bool HasLargeGuildOverride { get; private set; }
    public bool IsLargeGuild { get; private set; }

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
                        data.AnimatedIcon = true;
                        break;
                    case "ANIMATED_BANNER":
                        data.AnimatedBanner = true;
                        break;
                    case "AUDIO_BITRATE_128_KBPS":
                        data.AudioBitrate128Kbps = true;
                        break;
                    case "AUDIO_BITRATE_256_KBPS":
                        data.AudioBitrate256Kbps = true;
                        break;
                    case "AUDIO_BITRATE_384_KBPS":
                        data.AudioBitrate384Kbps = true;
                        break;
                    case "BANNER":
                        data.Banner = true;
                        break;
                    case "CLONE_EMOJI_ENABLED":
                        data.CloneEmoji = true;
                        break;
                    case "CLONE_STICKER_ENABLED":
                        data.CloneSticker = true;
                        break;
                    case "DETACHED_BANNER":
                        data.IsBannerDetached = true;
                        break;
                    case "INVITE_SPLASH":
                        data.InviteSplash = true;
                        break;
                    case "INVITES_DISABLED":
                        data.IsInvitesDisabled = true;
                        break;
                    case "RAID_DETECTED":
                        data.RaidDetected = true;
                        break;
                    case "TEXT_CHANNEL_FLEXIBLE_NAMES":
                        data.IsChannelNamesFlexible = true;
                        break;
                    case "HIDE_OWNER_CROWN":
                        data.HideOwnerCrown = true;
                        break;
                    case "MORE_EMOJI":
                        data.MoreEmojis = true;
                        break;
                    case "MORE_STICKERS":
                        data.MoreStickers = true;
                        break;
                    case "UNLIMITED_EMOJI":
                        data.UnlimitedEmojis = true;
                        break;
                    case "UNLIMITED_STICKERS":
                        data.UnlimitedStickers = true;
                        break;
                    case "EXPRESSION_PURGE_ALLOWED":
                        data.IsExpressionPurgeAllowed = true;
                        break;
                    case "VANITY_URL":
                        data.VanityUrl = true;
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
                    case "VOICE_E2EE":
                        data.VoiceE2EE = true;
                        break;
                    case "VIP_VOICE":
                        data.VipVoice = true;
                        break;
                    case "UNAVAILABLE_FOR_EVERYONE":
                        data.IsUnavailable = true;
                        break;
                    case "UNAVAILABLE_FOR_EVERYONE_BUT_STAFF":
                        data.IsStaffOnly = true;
                        break;
                    case "UNAVAILABLE_HIDDEN":
                        data.IsHidden = true;
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
                        data.IsLargeGuild = true;
                        break;
                }
            }
        }

        return data;
    }

    public static GuildFeatures FromGuild(PartialGuildJson guild)
    {
        return GuildFeatures.Create(guild.Features);
    }
}

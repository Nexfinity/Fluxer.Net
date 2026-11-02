namespace Fluxer.Net;

/// <summary>
/// The feature flags of a guild, read from its feature names.
/// </summary>
public class GuildFeatures
{
    /// <summary>
    /// The feature names of the guild as sent by Fluxer.
    /// </summary>
    public string[] Raw { get; private set; }

    /// <summary>
    /// Whether the guild has the ANIMATED_ICON feature.
    /// </summary>
    public bool AnimatedIcon { get; private set; }

    /// <summary>
    /// Whether the guild has the ANIMATED_BANNER feature.
    /// </summary>
    public bool AnimatedBanner { get; private set; }

    /// <summary>
    /// Whether the guild has the CLONE_EMOJI_ENABLED feature.
    /// </summary>
    public bool CloneEmoji { get; private set; }

    /// <summary>
    /// Whether the guild has the CLONE_STICKER_ENABLED feature.
    /// </summary>
    public bool CloneSticker { get; private set; }

    /// <summary>
    /// Whether the guild has the AUDIO_BITRATE_128_KBPS feature.
    /// </summary>
    public bool AudioBitrate128Kbps { get; private set; }

    /// <summary>
    /// Whether the guild has the AUDIO_BITRATE_256_KBPS feature.
    /// </summary>
    public bool AudioBitrate256Kbps { get; private set; }

    /// <summary>
    /// Whether the guild has the AUDIO_BITRATE_384_KBPS feature.
    /// </summary>
    public bool AudioBitrate384Kbps { get; private set; }

    /// <summary>
    /// Whether the guild has the BANNER feature.
    /// </summary>
    public bool Banner { get; private set; }

    /// <summary>
    /// Whether the guild has the DETACHED_BANNER feature.
    /// </summary>
    public bool IsBannerDetached { get; private set; }

    /// <summary>
    /// Whether the guild has the INVITE_SPLASH feature.
    /// </summary>
    public bool InviteSplash { get; private set; }

    /// <summary>
    /// Whether the guild has the INVITES_DISABLED feature.
    /// </summary>
    public bool IsInvitesDisabled { get; private set; }

    /// <summary>
    /// Whether the guild has the RAID_DETECTED feature.
    /// </summary>
    public bool RaidDetected { get; private set; }

    /// <summary>
    /// Whether the guild has the TEXT_CHANNEL_FLEXIBLE_NAMES feature.
    /// </summary>
    public bool IsChannelNamesFlexible { get; private set; }

    /// <summary>
    /// Whether the guild has the HIDE_OWNER_CROWN feature.
    /// </summary>
    public bool HideOwnerCrown { get; private set; }

    /// <summary>
    /// Whether the guild has the MORE_EMOJI feature.
    /// </summary>
    public bool MoreEmojis { get; private set; }

    /// <summary>
    /// Whether the guild has the MORE_STICKERS feature.
    /// </summary>
    public bool MoreStickers { get; private set; }

    /// <summary>
    /// Whether the guild has the UNLIMITED_EMOJI feature.
    /// </summary>
    public bool UnlimitedEmojis { get; private set; }

    /// <summary>
    /// Whether the guild has the UNLIMITED_STICKERS feature.
    /// </summary>
    public bool UnlimitedStickers { get; private set; }

    /// <summary>
    /// Whether the guild has the EXPRESSION_PURGE_ALLOWED feature.
    /// </summary>
    public bool IsExpressionPurgeAllowed { get; private set; }

    /// <summary>
    /// Whether the guild has the VANITY_URL feature.
    /// </summary>
    public bool VanityUrl { get; private set; }

    /// <summary>
    /// Whether the guild has the DISCOVERABLE feature.
    /// </summary>
    public bool IsDiscoverable { get; private set; }

    /// <summary>
    /// Whether the guild has the PARTNERED feature.
    /// </summary>
    public bool IsPartnered { get; private set; }

    /// <summary>
    /// Whether the guild has the VERIFIED feature.
    /// </summary>
    public bool IsVerified { get; private set; }

    /// <summary>
    /// Whether the guild has the VOICE_E2EE feature.
    /// </summary>
    public bool VoiceE2EE { get; private set; }

    /// <summary>
    /// Whether the guild has the VIP_VOICE feature.
    /// </summary>
    public bool VipVoice { get; private set; }

    /// <summary>
    /// Whether the guild has the UNAVAILABLE_FOR_EVERYONE feature.
    /// </summary>
    public bool IsUnavailable { get; private set; }

    /// <summary>
    /// Whether the guild has the UNAVAILABLE_HIDDEN feature.
    /// </summary>
    public bool IsHidden { get; private set; }

    /// <summary>
    /// Whether the guild has the UNAVAILABLE_FOR_EVERYONE_BUT_STAFF feature.
    /// </summary>
    public bool IsStaffOnly { get; private set; }

    /// <summary>
    /// Whether the guild has the VISIONARY feature.
    /// </summary>
    public bool IsVisionary { get; private set; }

    /// <summary>
    /// Whether the guild has the OPERATOR feature.
    /// </summary>
    public bool IsOperator { get; private set; }

    /// <summary>
    /// Whether the guild has the DISALLOW_UNCLAIMED_ACCOUNTS feature.
    /// </summary>
    public bool BlockUnclaimedAccounts { get; private set; }

    /// <summary>
    /// Whether the guild has the LARGE_GUILD_OVERRIDE feature.
    /// </summary>
    public bool HasLargeGuildOverride { get; private set; }

    /// <summary>
    /// Whether the guild has the VERY_LARGE_GUILD feature.
    /// </summary>
    public bool IsLargeGuild { get; private set; }

    /// <summary>
    /// Whether the guild has the ANNOUNCEMENT_CHANNELS_DISABLED feature.
    /// </summary>
    public bool IsAnnouncementChannelsDisabled { get; private set; }

    /// <summary>
    /// Name of the feature that blocks new invites from being used.
    /// </summary>
    public const string InvitesDisabledFeature = "INVITES_DISABLED";

    internal GuildFeatures()
    {

    }

    /// <summary>
    /// Creates the feature flags from a list of feature names.
    /// </summary>
    /// <param name="features">The feature names of the guild.</param>
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
                    case "ANNOUNCEMENT_CHANNELS_DISABLED":
                        data.IsAnnouncementChannelsDisabled = true;
                        break;
                }
            }
        }

        return data;
    }

    internal static string[]? WithFeature(string[]? current, string feature, bool enabled)
    {
        List<string> features = (current ?? []).ToList();
        bool present = features.Contains(feature);
        if (present == enabled)
            return null;

        if (enabled)
            features.Add(feature);
        else
            features.RemoveAll(x => x == feature);

        return features.ToArray();
    }

    /// <summary>
    /// Reads the feature flags from the features of a guild.
    /// </summary>
    /// <param name="guild">The guild whose features are read.</param>
    public static GuildFeatures FromGuild(PartialGuildJson guild)
    {
        return GuildFeatures.Create(guild.Features);
    }
}

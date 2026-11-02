namespace Fluxer.Net;

/// <summary>
/// A guild with its name, media and features.
/// </summary>
public interface IPartialGuild : ISnowflake
{
    /// <summary>
    /// The name of the guild.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The hash of the guild icon.
    /// </summary>
    string? IconHash { get; }

    /// <summary>
    /// The hash of the guild banner.
    /// </summary>
    string? BannerHash { get; }

    /// <summary>
    /// The width of the guild banner in pixels.
    /// </summary>
    int? BannerWidth { get; }

    /// <summary>
    /// The height of the guild banner in pixels.
    /// </summary>
    int? BannerHeight { get; }

    /// <summary>
    /// The hash of the embedded invite splash.
    /// </summary>
    string? EmbedSplashHash { get; }

    /// <summary>
    /// The width of the embedded invite splash in pixels.
    /// </summary>
    int? EmbedSplashWidth { get; }

    /// <summary>
    /// The height of the embedded invite splash in pixels.
    /// </summary>
    int? EmbedSplashHeight { get; }

    /// <summary>
    /// The hash of the guild splash screen.
    /// </summary>
    string? InviteSplashHash { get; }

    /// <summary>
    /// The width of the guild splash in pixels.
    /// </summary>
    int? InviteSplashWidth { get; }

    /// <summary>
    /// The height of the guild splash in pixels.
    /// </summary>
    int? InviteSplashHeight { get; }

    /// <summary>
    /// The alignment of the splash card.
    /// </summary>
    GuildSplashCardAlignment SplashCardAligment { get; }

    /// <summary>
    /// The feature flags enabled for the guild.
    /// </summary>
    /// <remarks>
    /// ANIMATED_ICON, ANIMATED_BANNER, BANNER, DETACHED_BANNER, INVITE_SPLASH, INVITES_DISABLED,
    /// TEXT_CHANNEL_FLEXIBLE_NAMES, MORE_EMOJI, MORE_STICKERS, UNLIMITED_EMOJI, UNLIMITED_STICKERS,
    /// EXPRESSION_PURGE_ALLOWED, VANITY_URL, VERIFIED, VIP_VOICE, UNAVAILABLE_FOR_EVERYONE,
    /// UNAVAILABLE_FOR_EVERYONE_BUT_STAFF, VISIONARY, OPERATOR, LARGE_GUILD_OVERRIDE, VERY_LARGE_GUILD (other values allowed)
    /// </remarks>
    string[]? Features { get; }

    /// <summary>
    /// The IDs of the badges assigned to the guild, as listed in the guild badges of the instance discovery document.
    /// </summary>
    ulong[]? BadgeIds { get; }

    /// <summary>
    /// Gets the URL of the guild's icon, or null when the guild has no icon.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string? GetIconUrl(int size);

    /// <summary>
    /// Gets the URL of the guild's banner, or null when the guild has no banner.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string? GetBannerUrl(int size);

    /// <summary>
    /// Gets the URL of the guild's invite splash, or null when the guild has none.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string? GetInviteSplashUrl(int size);

    /// <summary>
    /// Gets the URL of the guild's embed splash, or null when the guild has none.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string? GetEmbedSplashUrl(int size);
}

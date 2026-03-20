namespace Fluxer.Net;

public interface IPartialGuild
{
    /// <summary>
    /// The unique identifier for this guild.
    /// </summary>
    ulong Id { get; }

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
    /// The height of the embedded invite splash in pixels
    /// </summary>
    int? EmbedSplashHeight { get; }

    /// <summary>
    /// The hash of the guild splash screen.
    /// </summary>
    string? SplashHash { get; }

    /// <summary>
    /// The width of the guild splash in pixels.
    /// </summary>
    int? SplashWidth { get; }

    /// <summary>
    /// The height of the guild splash in pixels
    /// </summary>
    int? SplashHeight { get; }

    /// <summary>
    /// The alignment of the splash card.
    /// </summary>
    GuildSplashCardAlignment SplashCardAligment { get; }

    /// <summary>
    /// Array of guild feature flags.
    /// </summary>
    /// <remarks>
    /// ANIMATED_ICON, ANIMATED_BANNER, BANNER, DETACHED_BANNER, INVITE_SPLASH, INVITES_DISABLED,
    /// TEXT_CHANNEL_FLEXIBLE_NAMES, MORE_EMOJI, MORE_STICKERS, UNLIMITED_EMOJI, UNLIMITED_STICKERS,
    /// EXPRESSION_PURGE_ALLOWED, VANITY_URL, VERIFIED, VIP_VOICE, UNAVAILABLE_FOR_EVERYONE,
    /// UNAVAILABLE_FOR_EVERYONE_BUT_STAFF, VISIONARY, OPERATOR, LARGE_GUILD_OVERRIDE, VERY_LARGE_GUILD (other values allowed)
    /// </remarks>
    HashSet<string>? Features { get; }
}

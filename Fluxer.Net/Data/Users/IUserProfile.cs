namespace Fluxer.Net;

/// <summary>
/// The profile fields of a user, such as the bio and the banner.
/// </summary>
public interface IUserProfile
{
    /// <summary>
    /// The user biography text.
    /// </summary>
    string? Bio { get; }

    /// <summary>
    /// The preferred pronouns of the user.
    /// </summary>
    string? Pronouns { get; }

    /// <summary>
    /// The user-selected accent color as an integer.
    /// </summary>
    int? AccentColor { get; }

    /// <summary>
    /// The hash of the user profile banner image.
    /// </summary>
    string? BannerHash { get; }

    /// <summary>
    /// Default banner color if no custom banner.
    /// </summary>
    int? BannerColor { get; }
}

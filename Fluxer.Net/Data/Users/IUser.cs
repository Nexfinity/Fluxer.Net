namespace Fluxer.Net;

/// <summary>
/// A user account on Fluxer.
/// </summary>
public interface IUser : IMentionable, ISnowflake
{
    /// <summary>
    /// The username of the user, not unique across the platform.
    /// </summary>
    string Username { get; }

    /// <summary>
    /// The discriminator tag of the user.
    /// </summary>
    string Discriminator { get; }

    /// <summary>
    /// The display name of the user, or null when none is set.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// The hash of the avatar image, or null when the user has none.
    /// </summary>
    string? AvatarHash { get; }

    /// <summary>
    /// The dominant avatar color of the user as an integer, or null when there is none.
    /// </summary>
    int? AvatarColor { get; }

    /// <summary>
    /// The public flags on the user account.
    /// </summary>
    UserFlags Flags { get; }

    /// <summary>
    /// Whether the user is a bot account.
    /// </summary>
    bool IsBot { get; }

    /// <summary>
    /// Whether the user is an official system user.
    /// </summary>
    bool IsSystem { get; }

    /// <summary>
    /// How the user wants to be mentioned in replies.
    /// </summary>
    MentionPreference MentionPreference { get; }

    /// <summary>
    /// Gets the display name of the user, or the username when there is none.
    /// </summary>
    string GetCurrentName();

    /// <summary>
    /// Gets the URL of the default avatar of the user.
    /// </summary>
    string GetDefaultAvatarUrl();

    /// <summary>
    /// Gets the URL of the user's avatar, or null when the user has none.
    /// </summary>
    /// <param name="size">The requested image size.</param>
    string? GetAvatarUrl(int size);

    /// <summary>
    /// Gets the URL of the user's avatar, or of the default avatar when the user has none.
    /// </summary>
    /// <param name="size">The requested image size.</param>
    string GetAvatarOrDefaultUrl(int size);
}

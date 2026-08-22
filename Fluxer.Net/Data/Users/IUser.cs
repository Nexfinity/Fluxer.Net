namespace Fluxer.Net;

public interface IUser
{
    /// <summary>
    /// Unique identifier (snowflake) for the object.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// User created at UTC.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Get the mention for the user.
    /// </summary>
    string Mention { get; }

    /// <summary>
    /// The username of the user, not unique across the platform.
    /// </summary>
    string Username { get; }

    /// <summary>
    /// The four-digit discriminator tag of the user.
    /// </summary>
    string Discriminator { get; }

    /// <summary>
    /// The display name of the user, if set.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// The hash of the user avatar image.
    /// </summary>
    string? AvatarHash { get; }

    /// <summary>
    /// The dominant avatar color of the user as an integer.
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
    /// Get the user's current display name or username.
    /// </summary>
    string GetCurrentName();

    /// <summary>
    /// Get the default avatar for the user.
    /// </summary>
    string GetDefaultAvatarUrl();

    /// <summary>
    /// Get the user's avatar.
    /// </summary>
    string? GetAvatarUrl(int size);

    /// <summary>
    /// Get the user's avatar or fallback to default.
    /// </summary>
    string GetAvatarOrDefaultUrl(int size);
}

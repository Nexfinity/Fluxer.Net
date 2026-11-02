namespace Fluxer.Net;

/// <summary>
/// A webhook that posts messages into a channel.
/// </summary>
public interface IWebhook : ISnowflake
{
    /// <summary>
    /// The secure token used to execute the webhook.
    /// </summary>
    /// <remarks>
    /// Null when the current user does not have access to the token.
    /// </remarks>
    string? Token { get; }

    /// <summary>
    /// The ID of the guild this webhook belongs to.
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    /// The ID of the channel this webhook posts to.
    /// </summary>
    ulong? ChannelId { get; }

    /// <summary>
    /// The user that created this webhook.
    /// </summary>
    IUser? Creator { get; }

    /// <summary>
    /// The display name of the webhook.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The hash of the webhook avatar image.
    /// </summary>
    string? AvatarHash { get; }

    /// <summary>
    /// The type of the webhook.
    /// </summary>
    WebhookType Type { get; }

    /// <summary>
    /// Source guild of the followed webhook.
    /// </summary>
    IPartialGuild? SourceGuild { get; }

    /// <summary>
    /// Source channel of the followed webhook.
    /// </summary>
    IPartialChannel? SourceChannel { get; }

    /// <summary>
    /// Gets the default avatar URL of the webhook.
    /// </summary>
    string GetDefaultAvatarUrl();

    /// <summary>
    /// Gets the avatar URL of the webhook, or null when it has no avatar.
    /// </summary>
    /// <param name="size">The requested avatar size.</param>
    string? GetAvatarUrl(int size);

    /// <summary>
    /// Gets the avatar URL of the webhook, or the default avatar URL when it has no avatar.
    /// </summary>
    /// <param name="size">The requested avatar size.</param>
    string GetAvatarOrDefaultUrl(int size);
}

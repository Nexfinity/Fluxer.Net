namespace Fluxer.Net;

public interface IWebhook
{
    /// <summary>
    /// The unique identifier (snowflake) for the webhook.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The secure token used to execute the webhook.
    /// </summary>
    /// <remarks>
    /// Will be missing if you don't have access.
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
    UserJson? Creator { get; }

    /// <summary>
    /// The display name of the webhook.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The hash of the webhook avatar image.
    /// </summary>
    string? AvatarHash { get; }

    /// <summary>
    /// Get the default avatar for the user.
    /// </summary>
    string GetDefaultAvatarUrl();

    /// <summary>
    /// Get the webhooks's avatar.
    /// </summary>
    string? GetAvatarUrl(int size);

    /// <summary>
    /// Get the webhooks's avatar or fallback to default.
    /// </summary>
    string GetAvatarOrDefaultUrl(int size);
}

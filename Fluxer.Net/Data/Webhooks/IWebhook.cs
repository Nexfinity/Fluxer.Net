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

    string Name { get; }

    /// <summary>
    /// The hash of the webhook avatar image.
    /// </summary>
    string? AvatarHash { get; }
}

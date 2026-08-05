namespace Fluxer.Net;

/// <inheritdoc />
public class Webhook : Entity, IWebhook
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string? Token { get; internal set; }

    /// <inheritdoc />
    public ulong? GuildId { get; internal set; }

    /// <inheritdoc />
    public ulong? ChannelId { get; internal set; }

    /// <inheritdoc />
    public User? Creator { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string? AvatarHash { get; internal set; }

    IUser? IWebhook.Creator => Creator;

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"{Client.Config.StaticUrl}/avatars/{Id % 6}.png";
    }

    /// <inheritdoc />
    public string? GetAvatarUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return null;

        return $"{Client.Config.MediaUrl}/avatars/{Id}/{AvatarHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string GetAvatarOrDefaultUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return GetDefaultAvatarUrl();

        return GetAvatarUrl(size);
    }

    internal Webhook(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Webhook object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Webhook Create(FluxerBaseClient client, WebhookJson json)
    {
        Webhook data = new Webhook(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, WebhookJson json)
    {
        Id = json.Id;
        Token = json.Token;
        GuildId = json.GuildId;
        ChannelId = json.ChannelId;
        if (json.Creator != null)
            Creator = User.Create(client, json.Creator);

        Name = json.Name;
        AvatarHash = json.AvatarHash;
    }
}

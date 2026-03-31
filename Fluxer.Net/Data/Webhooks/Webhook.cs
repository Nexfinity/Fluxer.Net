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
    public UserJson? Creator { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string? AvatarHash { get; internal set; }

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"https://fluxerstatic.com/avatars/{Id % 6}.png";
    }

    internal Webhook(FluxerBaseClient client) : base(client)
    {

    }

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
        Creator = json.Creator;
        Name = json.Name;
        AvatarHash = json.AvatarHash;
    }
}

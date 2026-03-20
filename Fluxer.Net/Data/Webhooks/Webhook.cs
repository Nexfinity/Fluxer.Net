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

    internal Webhook(BaseClient client) : base(client)
    {

    }

    public static Webhook Create(BaseClient client, WebhookJson json)
    {
        var data = new Webhook(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, WebhookJson json)
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

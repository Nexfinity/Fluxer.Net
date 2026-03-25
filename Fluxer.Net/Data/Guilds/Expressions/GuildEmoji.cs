namespace Fluxer.Net;

/// <inheritdoc />
public class GuildEmoji : Entity, IGuildEmoji
{
    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public bool IsAnimated { get; internal set; }

    /// <inheritdoc />
    public User? Creator { get; internal set; }

    internal GuildEmoji(BaseClient client) : base(client)
    {

    }

    public static GuildEmoji Create(BaseClient client, GuildEmojiJson json, ulong guildId)
    {
        var data = new GuildEmoji(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, GuildEmojiJson json)
    {
        Id = json.Id;
        Name = json.Name;
        IsAnimated = json.IsAnimated;
        Creator = json.Creator;
    }
}

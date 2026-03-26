namespace Fluxer.Net;

/// <inheritdoc />
public class Emoji : Entity, IEmoji
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public bool IsAnimated { get; internal set; }

    internal Emoji(FluxerBaseClient client) : base(client)
    {

    }

    public static Emoji Create(FluxerBaseClient client, EmojiJson json)
    {
        var data = new Emoji(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, EmojiJson json)
    {
        Id = json.Id;
        Name = json.Name;
        IsAnimated = json.IsAnimated;
    }
}

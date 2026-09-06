namespace Fluxer.Net;

/// <inheritdoc />
public class Sticker : Entity, ISticker
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public bool IsAnimated { get; private set; }

    /// <inheritdoc />
    public bool AllowCloning { get; private set; }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is Emoji emoji)
        {
            return Id == emoji.Id &&
                Name == emoji.Name &&
                AllowCloning == emoji.AllowCloning;
        }

        return base.Equals(obj);
    }

    internal Sticker(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Sticker object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Sticker Create(FluxerBaseClient client, StickerJson json)
    {
        Sticker data = new Sticker(client);
        data.Update(json);
        return data;
    }

    internal void Update(StickerJson json)
    {
        Id = json.Id;
        Name = json.Name;
        IsAnimated = json.IsAnimated;
        AllowCloning = json.AllowCloning;
    }
}

namespace Fluxer.Net;

/// <inheritdoc />
public class Sticker : Entity, ISticker
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public bool IsAnimated { get; internal set; }

    /// <inheritdoc />
    public bool AllowCloning { get; internal set; }

    internal Sticker(FluxerBaseClient client) : base(client)
    {

    }

    public static Sticker Create(FluxerBaseClient client, StickerJson json)
    {
        Sticker data = new Sticker(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, StickerJson json)
    {
        Id = json.Id;
        Name = json.Name;
        IsAnimated = json.IsAnimated;
        AllowCloning = json.AllowCloning;
    }
}

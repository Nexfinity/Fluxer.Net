namespace Fluxer.Net;

/// <inheritdoc />
public class FavoriteGif : Entity, IFavoriteGif
{
    /// <inheritdoc />
    public string Id { get; private set; }

    /// <inheritdoc />
    public ulong UserId { get; private set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public string[] Tags { get; private set; }

    /// <inheritdoc />
    public ulong AttachmentId { get; private set; }

    /// <inheritdoc />
    public string Filename { get; private set; }

    /// <inheritdoc />
    public string ContentType { get; private set; }

    /// <inheritdoc />
    public int Size { get; private set; }

    /// <inheritdoc />
    public string Url { get; private set; }

    /// <inheritdoc />
    public string? AltText { get; private set; }

    /// <inheritdoc />
    public string? ContentHash { get; private set; }

    /// <inheritdoc />
    public int? Width { get; private set; }

    /// <inheritdoc />
    public int? Height { get; private set; }

    /// <inheritdoc />
    public int? Duration { get; private set; }

    /// <inheritdoc />
    public bool IsGifVideo { get; private set; }

    /// <inheritdoc />
    public string? KlipySlug { get; private set; }

    /// <inheritdoc />
    public string? TenorSlugId { get; private set; }

    internal FavoriteGif(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a FavoriteGif object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static FavoriteGif Create(FluxerBaseClient client, FavoriteGifJson json)
    {
        FavoriteGif data = new FavoriteGif(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, FavoriteGifJson json)
    {
        Id = json.Id;
        UserId = json.UserId;
        Name = json.Name;
        Tags = json.Tags;
        AttachmentId = json.AttachmentId;
        Filename = json.Filename;
        ContentType = json.ContentType;
        Size = json.Size;
        Url = json.Url;
        AltText = json.AltText;
        ContentHash = json.ContentHash;
        Width = json.Width;
        Height = json.Height;
        Duration = json.Duration;
        IsGifVideo = json.IsGifVideo;
        KlipySlug = json.KlipySlug;
        TenorSlugId = json.TenorSlugId;
    }
}

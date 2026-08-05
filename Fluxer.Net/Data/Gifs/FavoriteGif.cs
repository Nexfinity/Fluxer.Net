namespace Fluxer.Net;

/// <inheritdoc />
public class FavoriteGif : Entity, IFavoriteGif
{
    /// <inheritdoc />
    public string Id { get; internal set; }

    /// <inheritdoc />
    public ulong UserId { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string[] Tags { get; internal set; }

    /// <inheritdoc />
    public ulong AttachmentId { get; internal set; }

    /// <inheritdoc />
    public string Filename { get; internal set; }

    /// <inheritdoc />
    public string ContentType { get; internal set; }

    /// <inheritdoc />
    public int Size { get; internal set; }

    /// <inheritdoc />
    public string Url { get; internal set; }

    /// <inheritdoc />
    public string? AltText { get; internal set; }

    /// <inheritdoc />
    public string? ContentHash { get; internal set; }

    /// <inheritdoc />
    public int? Width { get; internal set; }

    /// <inheritdoc />
    public int? Height { get; internal set; }

    /// <inheritdoc />
    public int? Duration { get; internal set; }

    /// <inheritdoc />
    public bool IsGifVideo { get; internal set; }

    /// <inheritdoc />
    public string? KlipySlug { get; internal set; }

    /// <inheritdoc />
    public string? TenorSlugId { get; internal set; }

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

namespace Fluxer.Net;

/// <inheritdoc />
public class Gif : Entity, IGif
{
    /// <inheritdoc />
    public string Id { get; internal set; }

    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc />
    public string Url { get; set; }

    /// <inheritdoc />
    public string Source { get; set; }

    /// <inheritdoc />
    public string ProxySource { get; set; }

    /// <inheritdoc />
    public int Width { get; set; }

    /// <inheritdoc />
    public int Height { get; set; }

    internal Gif(FluxerBaseClient client) : base(client)
    {

    }

    public static Gif Create(FluxerBaseClient client, GifJson json)
    {
        Gif data = new Gif(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GifJson json)
    {
        Id = json.Id;
        Title = json.Title;
        Url = json.Url;
        Source = json.Source;
        ProxySource = json.ProxySource;
        Width = json.Width;
        Height = json.Height;
    }
}
